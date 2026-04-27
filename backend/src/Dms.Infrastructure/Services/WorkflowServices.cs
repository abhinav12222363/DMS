using Dms.Application.Abstractions;
using Dms.Application.Common;
using Dms.Application.Workflows;
using Dms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dms.Infrastructure.Services;

public sealed class DealerPortalService : IDealerPortalService
{
    private readonly IWorkflowRepository _repository;

    public DealerPortalService(IWorkflowRepository repository) => _repository = repository;

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct)
    {
        var distributor = await _repository.GetDistributorAsync(request.DistributorId, ct) ?? throw new KeyNotFoundException("Distributor not found");
        var order = new Order
        {
            DistributorId = request.DistributorId,
            OrderDate = request.OrderDate,
            OrderNumber = $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Status = request.SubmitForApproval ? "Pending Approval" : "Draft"
        };

        foreach (var line in request.Items)
        {
            var item = await _repository.GetItemAsync(line.ItemId, ct) ?? throw new KeyNotFoundException("Item not found");
            if (line.Quantity < item.Moq) throw new InvalidOperationException($"MOQ validation failed for {item.Name}");

            var stock = await _repository.GetStockAsync(request.DistributorId, line.ItemId, ct);
            if ((stock?.Quantity ?? 0) < line.Quantity) throw new InvalidOperationException($"Stock not available for {item.Name}");

            var rate = line.Rate ?? item.BasePrice;
            var gross = rate * line.Quantity;
            var (discount, free, cashback, points) = await ApplySchemeAsync(item, line.Quantity, gross, ct);
            var net = gross - discount;

            order.Items.Add(new OrderItem
            {
                ItemId = line.ItemId,
                Item = item,
                Quantity = line.Quantity,
                Rate = rate,
                LineGross = gross,
                DiscountAmount = discount,
                LineNet = net,
                FreeQuantity = free,
                CashbackAmount = cashback,
                Points = points
            });

            order.GrossAmount += gross;
            order.DiscountAmount += discount;
            order.NetAmount += net;
        }

        if (distributor.OutstandingAmount + order.NetAmount > distributor.CreditLimit)
            throw new InvalidOperationException("Credit limit exceeded");

        if (request.SubmitForApproval)
        {
            order.Approvals.Add(new OrderApproval { LevelNo = 1, ApproverRole = "SalesPerson", Status = "Pending" });
            order.Approvals.Add(new OrderApproval { LevelNo = 2, ApproverRole = "Admin", Status = "Waiting" });
        }

        await _repository.AddOrderAsync(order, ct);
        await _repository.SaveChangesAsync(ct);
        return ToOrderResponse(order);
    }

    public async Task<OrderResponse> CopyOrderAsync(Guid orderId, DateOnly newDate, CancellationToken ct)
    {
        var source = await _repository.GetOrderAsync(orderId, ct) ?? throw new KeyNotFoundException("Order not found");
        var create = new CreateOrderRequest(source.DistributorId, newDate, source.Items.Select(x => new OrderItemRequest(x.ItemId, x.Quantity, x.Rate)).ToList(), false);
        return await CreateOrderAsync(create, ct);
    }

    public async Task<PagedResult<OrderResponse>> GetOrdersAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = _repository.OrdersQuery().Where(x => x.DistributorId == distributorId).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.LongCountAsync(ct);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<OrderResponse>(items.Select(ToOrderResponse).ToList(), pageNumber, pageSize, total);
    }

    private async Task<(decimal discount, decimal free, decimal cashback, decimal points)> ApplySchemeAsync(Item item, decimal qty, decimal gross, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var schemes = await _repository.SchemesQuery().Where(x => x.Status == "Approved" && x.ValidFrom <= today && x.ValidTo >= today).ToListAsync(ct);
        foreach (var scheme in schemes)
        {
            var matched = scheme.ApplyOn switch
            {
                "Item" => scheme.ItemId == item.Id,
                "Item Group" => scheme.ItemGroup == item.Group,
                "Attribute" => scheme.Attribute == item.Attribute,
                _ => false
            };

            if (!matched) continue;
            var slab = scheme.Slabs.FirstOrDefault(s => qty >= s.MinQty && (s.MaxQty == null || qty <= s.MaxQty) && gross >= s.MinValue && (s.MaxValue == null || gross <= s.MaxValue));
            if (slab is null) continue;
            var discount = gross * slab.DiscountPercent / 100m;
            return (decimal.Round(discount, 2), slab.FreeQty, slab.CashbackAmount, slab.Points);
        }

        return (0, 0, 0, 0);
    }

    private static OrderResponse ToOrderResponse(Order order) => new(
        order.Id,
        order.OrderNumber,
        order.Status,
        order.GrossAmount,
        order.DiscountAmount,
        order.NetAmount,
        order.Items.Select(i => new OrderLineResponse(i.ItemId, i.Item.Name, i.Quantity, i.Rate, i.DiscountAmount, i.FreeQuantity, i.CashbackAmount, i.Points, i.LineNet)).ToList());
}

public sealed class ApprovalService : IApprovalService
{
    private readonly IWorkflowRepository _repository;
    private readonly IEmailSender _emailSender;

    public ApprovalService(IWorkflowRepository repository, IEmailSender emailSender)
    {
        _repository = repository;
        _emailSender = emailSender;
    }

    public async Task<OrderResponse> ApproveOrderAsync(Guid orderId, OrderApprovalActionRequest request, CancellationToken ct)
    {
        var order = await _repository.GetOrderAsync(orderId, ct) ?? throw new KeyNotFoundException("Order not found");
        var current = order.Approvals.OrderBy(x => x.LevelNo).FirstOrDefault(x => x.Status == "Pending") ?? throw new InvalidOperationException("No pending approval level");
        current.Status = "Approved";
        current.ActionByUserId = request.UserId;
        current.ActionAtUtc = DateTime.UtcNow;
        current.Remarks = request.Remarks;

        var next = order.Approvals.FirstOrDefault(x => x.LevelNo == current.LevelNo + 1);
        if (next is not null)
        {
            next.Status = "Pending";
            order.Status = "Pending Approval";
        }
        else
        {
            order.Status = "Approved";
            var invoice = new Invoice
            {
                OrderId = order.Id,
                DistributorId = order.DistributorId,
                InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Amount = order.NetAmount
            };
            await _repository.AddInvoiceAsync(invoice, ct);

            var distributor = await _repository.GetDistributorAsync(order.DistributorId, ct) ?? throw new KeyNotFoundException("Distributor missing");
            distributor.OutstandingAmount += order.NetAmount;

            foreach (var line in order.Items)
            {
                var stock = await _repository.GetStockAsync(order.DistributorId, line.ItemId, ct);
                if (stock is null) throw new InvalidOperationException("Stock row missing");
                stock.Quantity -= line.Quantity;
            }

            await _emailSender.SendAsync("ops@dms.local", "Order Approved", $"{order.OrderNumber} approved and invoiced", ct);
        }

        await _repository.SaveChangesAsync(ct);
        return new OrderResponse(order.Id, order.OrderNumber, order.Status, order.GrossAmount, order.DiscountAmount, order.NetAmount,
            order.Items.Select(i => new OrderLineResponse(i.ItemId, i.Item.Name, i.Quantity, i.Rate, i.DiscountAmount, i.FreeQuantity, i.CashbackAmount, i.Points, i.LineNet)).ToList());
    }

    public async Task<OrderResponse> RejectOrderAsync(Guid orderId, OrderApprovalActionRequest request, CancellationToken ct)
    {
        var order = await _repository.GetOrderAsync(orderId, ct) ?? throw new KeyNotFoundException("Order not found");
        var current = order.Approvals.OrderBy(x => x.LevelNo).FirstOrDefault(x => x.Status is "Pending" or "Waiting") ?? throw new InvalidOperationException("No actionable approval level");
        current.Status = "Rejected";
        current.ActionByUserId = request.UserId;
        current.ActionAtUtc = DateTime.UtcNow;
        current.Remarks = request.Remarks;
        order.Status = "Rejected";
        await _repository.SaveChangesAsync(ct);
        return new OrderResponse(order.Id, order.OrderNumber, order.Status, order.GrossAmount, order.DiscountAmount, order.NetAmount,
            order.Items.Select(i => new OrderLineResponse(i.ItemId, i.Item.Name, i.Quantity, i.Rate, i.DiscountAmount, i.FreeQuantity, i.CashbackAmount, i.Points, i.LineNet)).ToList());
    }
}

public sealed class SchemeWorkflowService : ISchemeWorkflowService
{
    private readonly IWorkflowRepository _repository;
    public SchemeWorkflowService(IWorkflowRepository repository) => _repository = repository;

    public async Task<SchemeResponse> CreateAsync(CreateSchemeRequest request, CancellationToken ct)
    {
        var entity = new Scheme
        {
            Name = request.Name,
            SchemeType = request.SchemeType,
            ApplyOn = request.ApplyOn,
            CalculationMode = request.CalculationMode,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            ItemId = request.ItemId,
            ItemGroup = request.ItemGroup,
            Attribute = request.Attribute,
            Status = "Pending Approval",
            Slabs = request.Slabs.Select(s => new SchemeSlab
            {
                MinQty = s.MinQty,
                MaxQty = s.MaxQty,
                MinValue = s.MinValue,
                MaxValue = s.MaxValue,
                DiscountPercent = s.DiscountPercent,
                FreeQty = s.FreeQty,
                CashbackAmount = s.CashbackAmount,
                Points = s.Points
            }).ToList()
        };

        await _repository.AddSchemeAsync(entity, ct);
        await _repository.SaveChangesAsync(ct);
        return new SchemeResponse(entity.Id, entity.Name, entity.Status, entity.SchemeType);
    }

    public async Task<SchemeResponse> ApproveAsync(Guid schemeId, CancellationToken ct)
    {
        var scheme = await _repository.GetSchemeAsync(schemeId, ct) ?? throw new KeyNotFoundException("Scheme not found");
        scheme.Status = "Approved";
        await _repository.SaveChangesAsync(ct);
        return new SchemeResponse(scheme.Id, scheme.Name, scheme.Status, scheme.SchemeType);
    }

    public async Task<IReadOnlyCollection<SchemeResponse>> GetAsync(CancellationToken ct)
    {
        var list = await _repository.SchemesQuery().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);
        return list.Select(x => new SchemeResponse(x.Id, x.Name, x.Status, x.SchemeType)).ToList();
    }
}

public sealed class ClaimService : IClaimService
{
    private readonly IWorkflowRepository _repository;
    public ClaimService(IWorkflowRepository repository) => _repository = repository;

    public async Task<ClaimResponse> CreateAsync(CreateClaimRequest request, CancellationToken ct)
    {
        var claim = new Claim
        {
            DistributorId = request.DistributorId,
            ClaimType = request.ClaimType,
            Amount = request.Amount,
            Reason = request.Reason,
            Status = "Pending",
            Documents = request.Documents.Select(x => new ClaimDocument { FileName = x, FileUrl = $"/docs/{x}" }).ToList()
        };

        await _repository.AddClaimAsync(claim, ct);
        await _repository.SaveChangesAsync(ct);
        return new ClaimResponse(claim.Id, claim.ClaimType, claim.Amount, claim.Status, claim.Reason);
    }

    public async Task<ClaimResponse> ApproveAsync(Guid claimId, CancellationToken ct)
    {
        var claim = await _repository.GetClaimAsync(claimId, ct) ?? throw new KeyNotFoundException("Claim not found");
        claim.Status = "Approved";
        var distributor = await _repository.GetDistributorAsync(claim.DistributorId, ct) ?? throw new KeyNotFoundException("Distributor not found");
        distributor.OutstandingAmount -= claim.Amount;
        await _repository.SaveChangesAsync(ct);
        return new ClaimResponse(claim.Id, claim.ClaimType, claim.Amount, claim.Status, claim.Reason);
    }

    public async Task<ClaimResponse> RejectAsync(Guid claimId, CancellationToken ct)
    {
        var claim = await _repository.GetClaimAsync(claimId, ct) ?? throw new KeyNotFoundException("Claim not found");
        claim.Status = "Rejected";
        await _repository.SaveChangesAsync(ct);
        return new ClaimResponse(claim.Id, claim.ClaimType, claim.Amount, claim.Status, claim.Reason);
    }

    public async Task<PagedResult<ClaimResponse>> GetAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = _repository.ClaimsQuery().Where(x => x.DistributorId == distributorId).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.LongCountAsync(ct);
        var claims = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<ClaimResponse>(claims.Select(c => new ClaimResponse(c.Id, c.ClaimType, c.Amount, c.Status, c.Reason)).ToList(), pageNumber, pageSize, total);
    }
}

public sealed class InventoryService : IInventoryService
{
    private readonly IWorkflowRepository _repository;
    public InventoryService(IWorkflowRepository repository) => _repository = repository;

    public async Task<StockResponse> AdjustAsync(StockAdjustmentRequest request, CancellationToken ct)
    {
        var stock = await _repository.GetStockAsync(request.DistributorId, request.ItemId, ct);
        if (stock is null)
        {
            var item = await _repository.GetItemAsync(request.ItemId, ct) ?? throw new KeyNotFoundException("Item not found");
            stock = new Stock { DistributorId = request.DistributorId, ItemId = request.ItemId, Quantity = 0, ReorderLevel = item.Moq * 2, Item = item };
            await _repository.AddStockAsync(stock, ct);
        }

        stock.Quantity += request.TransactionType == "Increase" ? request.Quantity : -request.Quantity;
        await _repository.SaveChangesAsync(ct);
        return new StockResponse(stock.DistributorId, stock.ItemId, stock.Item.Name, stock.Quantity, stock.ReorderLevel);
    }

    public async Task<IReadOnlyCollection<StockResponse>> GetStockAsync(Guid distributorId, CancellationToken ct)
    {
        var stocks = await _repository.StocksQuery().Where(x => x.DistributorId == distributorId).ToListAsync(ct);
        return stocks.Select(x => new StockResponse(x.DistributorId, x.ItemId, x.Item.Name, x.Quantity, x.ReorderLevel)).ToList();
    }
}

public sealed class ReplenishmentService : IReplenishmentService
{
    private readonly IWorkflowRepository _repository;
    public ReplenishmentService(IWorkflowRepository repository) => _repository = repository;

    public async Task RunAutoSuggestAsync(CancellationToken ct)
    {
        var lowStocks = await _repository.StocksQuery().Where(x => x.Quantity < x.ReorderLevel).ToListAsync(ct);
        foreach (var stock in lowStocks)
        {
            var suggestedQty = stock.ReorderLevel - stock.Quantity;
            await _repository.AddReplenishmentAsync(new Replenishment
            {
                DistributorId = stock.DistributorId,
                ItemId = stock.ItemId,
                CurrentStock = stock.Quantity,
                SuggestedQty = suggestedQty,
                Status = "Suggested"
            }, ct);
        }

        await _repository.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<object>> GetSuggestionsAsync(Guid distributorId, CancellationToken ct)
    {
        var rows = await _repository.StocksQuery().Where(x => x.DistributorId == distributorId && x.Quantity < x.ReorderLevel)
            .Select(x => (object)new { x.ItemId, ItemName = x.Item.Name, x.Quantity, x.ReorderLevel, SuggestedQty = x.ReorderLevel - x.Quantity })
            .ToListAsync(ct);
        return rows;
    }
}

public sealed class ErpIntegrationService : IErpIntegrationService
{
    private readonly IWorkflowRepository _repository;
    private const string StaticToken = "ERP-SYNC-TOKEN";

    public ErpIntegrationService(IWorkflowRepository repository) => _repository = repository;

    public Task<string> IssueTokenAsync(ErpTokenRequest request, CancellationToken ct)
    {
        if (request.ApiKey != "DMS-ERP-KEY") throw new UnauthorizedAccessException("Invalid API key");
        return Task.FromResult(StaticToken);
    }

    public async Task UpsertItemAsync(ErpItemUpsertRequest request, string token, CancellationToken ct)
    {
        ValidateToken(token);
        var item = await _repository.GetItemByCodeAsync(request.ItemCode, ct);
        if (item is null)
        {
            item = new Item { ItemCode = request.ItemCode, Name = request.Name, Unit = request.Unit, Group = request.Group, Attribute = request.Attribute, Moq = request.Moq, BasePrice = request.BasePrice };
            await _repository.AddItemAsync(item, ct);
        }

        item.Name = request.Name;
        item.Unit = request.Unit;
        item.Group = request.Group;
        item.Attribute = request.Attribute;
        item.Moq = request.Moq;
        item.BasePrice = request.BasePrice;
        await _repository.SaveChangesAsync(ct);
    }

    public async Task UpsertDistributorAsync(ErpDistributorUpsertRequest request, string token, CancellationToken ct)
    {
        ValidateToken(token);
        var distributor = await _repository.GetDistributorByCodeAsync(request.Code, ct);
        if (distributor is null)
        {
            distributor = new Distributor { Code = request.Code, Name = request.Name, Zone = request.Zone, CreditLimit = request.CreditLimit, OutstandingAmount = 0, IsActive = true };
            await _repository.AddDistributorAsync(distributor, ct);
        }
        distributor.Name = request.Name;
        distributor.Zone = request.Zone;
        distributor.CreditLimit = request.CreditLimit;
        await _repository.SaveChangesAsync(ct);
    }

    public Task UpsertHsnAsync(ErpHsnUpsertRequest request, string token, CancellationToken ct)
    {
        ValidateToken(token);
        return Task.CompletedTask;
    }

    public async Task PushSalesInvoiceAsync(ErpInvoicePushRequest request, string token, CancellationToken ct)
    {
        ValidateToken(token);
        var order = await _repository.OrdersQuery().FirstOrDefaultAsync(x => x.OrderNumber == request.OrderNumber, ct) ?? throw new KeyNotFoundException("Order missing");
        await _repository.AddInvoiceAsync(new Invoice
        {
            OrderId = order.Id,
            DistributorId = order.DistributorId,
            InvoiceNumber = request.InvoiceNumber,
            InvoiceDate = request.InvoiceDate,
            Amount = request.Amount,
            SyncStatus = "Synced"
        }, ct);
        await _repository.SaveChangesAsync(ct);
    }

    private static void ValidateToken(string token)
    {
        if (token != StaticToken) throw new UnauthorizedAccessException("Invalid token");
    }
}
