using Dms.Application.Abstractions;
using Dms.Application.Reports;

namespace Dms.Infrastructure.Services;

public interface IReportService
{
    Task<IReadOnlyCollection<SalesReportRow>> GetSalesAsync(ReportFilter filter, CancellationToken ct);
}

public sealed class ReportService : IReportService
{
    private readonly ISalesOrderRepository _salesOrderRepository;

    public ReportService(ISalesOrderRepository salesOrderRepository) => _salesOrderRepository = salesOrderRepository;

    public Task<IReadOnlyCollection<SalesReportRow>> GetSalesAsync(ReportFilter filter, CancellationToken ct) =>
        _salesOrderRepository.GetSalesReportAsync(filter, ct, readOnlyReplica: true);
}
