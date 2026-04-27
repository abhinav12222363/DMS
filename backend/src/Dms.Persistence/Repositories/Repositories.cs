using Dms.Application.Abstractions;
using Dms.Application.Common;
using Dms.Application.Dashboard;
using Dms.Application.Distributors;
using Dms.Application.Reports;
using Dms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dms.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly DmsDbContext _db;
    public UserRepository(DmsDbContext db) => _db = db;

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct) => _db.Users.FirstOrDefaultAsync(x => x.Username == username, ct);
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) => _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) => _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class DistributorRepository : IDistributorRepository
{
    private readonly DmsDbContext _db;
    public DistributorRepository(DmsDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<DistributorDto>> GetForUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserDistributors
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new DistributorDto(x.Distributor.Id, x.Distributor.Code, x.Distributor.Name, x.Distributor.Zone))
            .ToListAsync(ct);
    }
}

public sealed class ItemRepository : IItemRepository
{
    private readonly DmsDbContext _db;
    public ItemRepository(DmsDbContext db) => _db = db;

    public async Task<PagedResult<Item>> GetPagedAsync(string? search, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = _db.Items.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search) || x.ItemCode.Contains(search));
        }

        var total = await query.LongCountAsync(ct);
        var items = await query.AsNoTracking().OrderBy(x => x.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Item>(items, pageNumber, pageSize, total);
    }

    public Task AddAsync(Item item, CancellationToken ct) => _db.Items.AddAsync(item, ct).AsTask();
    public Task<Item?> GetByIdAsync(Guid id, CancellationToken ct) => _db.Items.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task RemoveAsync(Item item, CancellationToken ct) { _db.Items.Remove(item); return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class SalesOrderRepository : ISalesOrderRepository
{
    private readonly DmsDbContext _writeDb;
    private readonly IReadOnlyDmsDbContextFactory _readOnlyFactory;

    public SalesOrderRepository(DmsDbContext writeDb, IReadOnlyDmsDbContextFactory readOnlyFactory)
    {
        _writeDb = writeDb;
        _readOnlyFactory = readOnlyFactory;
    }

    public async Task<PagedResult<SalesOrder>> GetPagedAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = _writeDb.SalesOrders.Where(x => x.DistributorId == distributorId);
        var total = await query.LongCountAsync(ct);
        var items = await query.AsNoTracking().OrderByDescending(x => x.OrderDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<SalesOrder>(items, pageNumber, pageSize, total);
    }

    public Task AddAsync(SalesOrder order, CancellationToken ct) => _writeDb.SalesOrders.AddAsync(order, ct).AsTask();
    public Task SaveChangesAsync(CancellationToken ct) => _writeDb.SaveChangesAsync(ct);

    public async Task<DashboardResponse> GetDashboardAsync(Guid distributorId, DateOnly fromDate, DateOnly toDate, CancellationToken ct)
    {
        var query = _writeDb.SalesOrders.AsNoTracking().Where(x => x.DistributorId == distributorId && x.OrderDate >= fromDate && x.OrderDate <= toDate);
        var totalSales = await query.SumAsync(x => x.TotalAmount, ct);
        var orders = await query.LongCountAsync(ct);

        var trend = await query.GroupBy(x => x.OrderDate)
            .Select(g => new ChartPointDto(g.Key.ToString("yyyy-MM-dd"), g.Sum(x => x.TotalAmount)))
            .OrderBy(x => x.Label)
            .ToListAsync(ct);

        return new DashboardResponse(
            new[] {
                new KpiCardDto("Sales Value", totalSales, 12.7m),
                new KpiCardDto("Orders", orders, 8.3m)
            },
            trend,
            new[] { new TopItemDto("Top SKU 1", totalSales * 0.2m), new TopItemDto("Top SKU 2", totalSales * 0.15m) });
    }

    public async Task<IReadOnlyCollection<SalesReportRow>> GetSalesReportAsync(ReportFilter filter, CancellationToken ct, bool readOnlyReplica = false)
    {
        var db = readOnlyReplica ? _readOnlyFactory.Create() : _writeDb;
        return await db.SalesOrders
            .AsNoTracking()
            .Where(x => x.DistributorId == filter.DistributorId && x.OrderDate >= filter.FromDate && x.OrderDate <= filter.ToDate)
            .GroupBy(x => x.Distributor.Zone)
            .Select(g => new SalesReportRow(g.Key, g.Sum(x => x.TotalAmount), g.LongCount()))
            .ToListAsync(ct);
    }
}

public interface IReadOnlyDmsDbContextFactory
{
    DmsDbContext Create();
}

public sealed class ReadOnlyDmsDbContextFactory : IReadOnlyDmsDbContextFactory
{
    private readonly DbContextOptions<DmsDbContext> _options;
    public ReadOnlyDmsDbContextFactory(DbContextOptions<DmsDbContext> options) => _options = options;

    public DmsDbContext Create() => new(_options);
}
