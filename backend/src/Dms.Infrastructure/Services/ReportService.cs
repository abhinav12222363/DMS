using Dms.Application.Abstractions;
using Dms.Application.Reports;
using Microsoft.EntityFrameworkCore;

namespace Dms.Infrastructure.Services;

public interface IReportService
{
    Task<IReadOnlyCollection<SalesReportRow>> GetSalesAsync(ReportFilter filter, CancellationToken ct);
    Task<IReadOnlyCollection<OrderReportRow>> GetOrdersAsync(Guid distributorId, CancellationToken ct);
    Task<IReadOnlyCollection<StockReportRow>> GetStockAsync(Guid distributorId, CancellationToken ct);
    Task<IReadOnlyCollection<ClaimReportRow>> GetClaimsAsync(Guid distributorId, CancellationToken ct);
    Task<IReadOnlyCollection<SchemeReportRow>> GetSchemesAsync(CancellationToken ct);
}

public sealed class ReportService : IReportService
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public ReportService(ISalesOrderRepository salesOrderRepository, IWorkflowRepository workflowRepository)
    {
        _salesOrderRepository = salesOrderRepository;
        _workflowRepository = workflowRepository;
    }

    public Task<IReadOnlyCollection<SalesReportRow>> GetSalesAsync(ReportFilter filter, CancellationToken ct) =>
        _salesOrderRepository.GetSalesReportAsync(filter, ct, readOnlyReplica: true);

    public async Task<IReadOnlyCollection<OrderReportRow>> GetOrdersAsync(Guid distributorId, CancellationToken ct)
        => await _workflowRepository.OrdersQuery().Where(x => x.DistributorId == distributorId)
            .GroupBy(x => x.Status)
            .Select(g => new OrderReportRow(g.Key, g.LongCount(), g.Sum(x => x.NetAmount)))
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<StockReportRow>> GetStockAsync(Guid distributorId, CancellationToken ct)
        => await _workflowRepository.StocksQuery().Where(x => x.DistributorId == distributorId)
            .Select(x => new StockReportRow(x.Item.Name, x.Quantity, x.ReorderLevel))
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<ClaimReportRow>> GetClaimsAsync(Guid distributorId, CancellationToken ct)
        => await _workflowRepository.ClaimsQuery().Where(x => x.DistributorId == distributorId)
            .GroupBy(x => x.ClaimType)
            .Select(g => new ClaimReportRow(g.Key, g.LongCount(), g.Sum(x => x.Amount)))
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<SchemeReportRow>> GetSchemesAsync(CancellationToken ct)
        => await _workflowRepository.SchemesQuery().Select(x => new SchemeReportRow(x.Name, x.SchemeType, x.Status)).ToListAsync(ct);
}
