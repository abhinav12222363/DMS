using Dms.Application.Abstractions;
using Dms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dms.Persistence.Repositories;

public sealed class WorkflowRepository : IWorkflowRepository
{
    private readonly DmsDbContext _db;
    public WorkflowRepository(DmsDbContext db) => _db = db;

    public Task<Distributor?> GetDistributorAsync(Guid distributorId, CancellationToken ct)
        => _db.Distributors.FirstOrDefaultAsync(x => x.Id == distributorId, ct);

    public Task<Order?> GetOrderAsync(Guid orderId, CancellationToken ct)
        => _db.Orders.Include(x => x.Items).Include(x => x.Approvals).FirstOrDefaultAsync(x => x.Id == orderId, ct);

    public Task<Item?> GetItemAsync(Guid itemId, CancellationToken ct)
        => _db.Items.FirstOrDefaultAsync(x => x.Id == itemId, ct);

    public Task<Item?> GetItemByCodeAsync(string itemCode, CancellationToken ct)
        => _db.Items.FirstOrDefaultAsync(x => x.ItemCode == itemCode, ct);

    public Task<Distributor?> GetDistributorByCodeAsync(string code, CancellationToken ct)
        => _db.Distributors.FirstOrDefaultAsync(x => x.Code == code, ct);

    public Task<Stock?> GetStockAsync(Guid distributorId, Guid itemId, CancellationToken ct)
        => _db.Stocks.Include(x => x.Item).FirstOrDefaultAsync(x => x.DistributorId == distributorId && x.ItemId == itemId, ct);

    public Task<Scheme?> GetSchemeAsync(Guid schemeId, CancellationToken ct)
        => _db.Schemes.Include(x => x.Slabs).FirstOrDefaultAsync(x => x.Id == schemeId, ct);

    public Task<Claim?> GetClaimAsync(Guid claimId, CancellationToken ct)
        => _db.Claims.FirstOrDefaultAsync(x => x.Id == claimId, ct);

    public IQueryable<Order> OrdersQuery() => _db.Orders.Include(x => x.Items).AsQueryable();
    public IQueryable<Scheme> SchemesQuery() => _db.Schemes.Include(x => x.Slabs).AsQueryable();
    public IQueryable<Claim> ClaimsQuery() => _db.Claims.AsQueryable();
    public IQueryable<Stock> StocksQuery() => _db.Stocks.Include(x => x.Item).AsQueryable();

    public Task AddOrderAsync(Order order, CancellationToken ct) => _db.Orders.AddAsync(order, ct).AsTask();
    public Task AddItemAsync(Item item, CancellationToken ct) => _db.Items.AddAsync(item, ct).AsTask();
    public Task AddDistributorAsync(Distributor distributor, CancellationToken ct) => _db.Distributors.AddAsync(distributor, ct).AsTask();
    public Task AddStockAsync(Stock stock, CancellationToken ct) => _db.Stocks.AddAsync(stock, ct).AsTask();
    public Task AddInvoiceAsync(Invoice invoice, CancellationToken ct) => _db.Invoices.AddAsync(invoice, ct).AsTask();
    public Task AddSchemeAsync(Scheme scheme, CancellationToken ct) => _db.Schemes.AddAsync(scheme, ct).AsTask();
    public Task AddClaimAsync(Claim claim, CancellationToken ct) => _db.Claims.AddAsync(claim, ct).AsTask();
    public Task AddReplenishmentAsync(Replenishment replenishment, CancellationToken ct) => _db.Replenishments.AddAsync(replenishment, ct).AsTask();
    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
