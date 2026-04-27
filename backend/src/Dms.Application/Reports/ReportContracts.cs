namespace Dms.Application.Reports;

public sealed record ReportFilter(DateOnly FromDate, DateOnly ToDate, Guid DistributorId);
public sealed record SalesReportRow(string Region, decimal SalesValue, long Orders);
