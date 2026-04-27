using Dms.Application.Common;

namespace Dms.Application.Workflows;

public sealed record OrderItemRequest(Guid ItemId, decimal Quantity, decimal? Rate);
public sealed record CreateOrderRequest(Guid DistributorId, DateOnly OrderDate, List<OrderItemRequest> Items, bool SubmitForApproval);
public sealed record OrderLineResponse(Guid ItemId, string ItemName, decimal Quantity, decimal Rate, decimal DiscountAmount, decimal FreeQuantity, decimal CashbackAmount, decimal Points, decimal LineNet);
public sealed record OrderResponse(Guid Id, string OrderNumber, string Status, decimal GrossAmount, decimal DiscountAmount, decimal NetAmount, IReadOnlyCollection<OrderLineResponse> Items);
public sealed record OrderApprovalActionRequest(Guid UserId, string Remarks);

public sealed record CreateSchemeSlabRequest(decimal MinQty, decimal? MaxQty, decimal MinValue, decimal? MaxValue, decimal DiscountPercent, decimal FreeQty, decimal CashbackAmount, decimal Points);
public sealed record CreateSchemeRequest(string Name, string SchemeType, string ApplyOn, string CalculationMode, DateOnly ValidFrom, DateOnly ValidTo, Guid? ItemId, string? ItemGroup, string? Attribute, List<CreateSchemeSlabRequest> Slabs);
public sealed record SchemeResponse(Guid Id, string Name, string Status, string SchemeType);

public sealed record CreateClaimRequest(Guid DistributorId, string ClaimType, decimal Amount, string Reason, List<string> Documents);
public sealed record ClaimResponse(Guid Id, string ClaimType, decimal Amount, string Status, string Reason);

public sealed record StockAdjustmentRequest(Guid DistributorId, Guid ItemId, decimal Quantity, string TransactionType);
public sealed record StockResponse(Guid DistributorId, Guid ItemId, string ItemName, decimal Quantity, decimal ReorderLevel);

public sealed record ErpTokenRequest(string ApiKey);
public sealed record ErpItemUpsertRequest(string ItemCode, string Name, string Unit, string Group, string Attribute, decimal Moq, decimal BasePrice);
public sealed record ErpDistributorUpsertRequest(string Code, string Name, string Zone, decimal CreditLimit);
public sealed record ErpHsnUpsertRequest(string HsnCode, decimal GstRate);
public sealed record ErpInvoicePushRequest(string InvoiceNumber, string OrderNumber, DateOnly InvoiceDate, decimal Amount);

public interface IDealerPortalService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct);
    Task<OrderResponse> CopyOrderAsync(Guid orderId, DateOnly newDate, CancellationToken ct);
    Task<PagedResult<OrderResponse>> GetOrdersAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct);
}

public interface IApprovalService
{
    Task<OrderResponse> ApproveOrderAsync(Guid orderId, OrderApprovalActionRequest request, CancellationToken ct);
    Task<OrderResponse> RejectOrderAsync(Guid orderId, OrderApprovalActionRequest request, CancellationToken ct);
}

public interface ISchemeWorkflowService
{
    Task<SchemeResponse> CreateAsync(CreateSchemeRequest request, CancellationToken ct);
    Task<SchemeResponse> ApproveAsync(Guid schemeId, CancellationToken ct);
    Task<IReadOnlyCollection<SchemeResponse>> GetAsync(CancellationToken ct);
}

public interface IClaimService
{
    Task<ClaimResponse> CreateAsync(CreateClaimRequest request, CancellationToken ct);
    Task<ClaimResponse> ApproveAsync(Guid claimId, CancellationToken ct);
    Task<ClaimResponse> RejectAsync(Guid claimId, CancellationToken ct);
    Task<PagedResult<ClaimResponse>> GetAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct);
}

public interface IInventoryService
{
    Task<StockResponse> AdjustAsync(StockAdjustmentRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<StockResponse>> GetStockAsync(Guid distributorId, CancellationToken ct);
}

public interface IReplenishmentService
{
    Task RunAutoSuggestAsync(CancellationToken ct);
    Task<IReadOnlyCollection<object>> GetSuggestionsAsync(Guid distributorId, CancellationToken ct);
}

public interface IErpIntegrationService
{
    Task<string> IssueTokenAsync(ErpTokenRequest request, CancellationToken ct);
    Task UpsertItemAsync(ErpItemUpsertRequest request, string token, CancellationToken ct);
    Task UpsertDistributorAsync(ErpDistributorUpsertRequest request, string token, CancellationToken ct);
    Task UpsertHsnAsync(ErpHsnUpsertRequest request, string token, CancellationToken ct);
    Task PushSalesInvoiceAsync(ErpInvoicePushRequest request, string token, CancellationToken ct);
}
