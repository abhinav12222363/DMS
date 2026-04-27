using Dms.Application.Abstractions;
using Dms.Application.Dashboard;

namespace Dms.Infrastructure.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetSalesDashboardAsync(Guid distributorId, DateOnly fromDate, DateOnly toDate, CancellationToken ct);
}

public sealed class DashboardService : IDashboardService
{
    private readonly ISalesOrderRepository _salesOrderRepository;

    public DashboardService(ISalesOrderRepository salesOrderRepository) => _salesOrderRepository = salesOrderRepository;

    public Task<DashboardResponse> GetSalesDashboardAsync(Guid distributorId, DateOnly fromDate, DateOnly toDate, CancellationToken ct) =>
        _salesOrderRepository.GetDashboardAsync(distributorId, fromDate, toDate, ct);
}
