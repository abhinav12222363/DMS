namespace Dms.Application.Reports;

public sealed record ReportFilter(DateOnly FromDate, DateOnly ToDate, Guid DistributorId);
public sealed record SalesReportRow(string Region, decimal SalesValue, long Orders);
public sealed record OrderReportRow(string Status, long Count, decimal NetAmount);
public sealed record StockReportRow(string ItemName, decimal Quantity, decimal ReorderLevel);
public sealed record ClaimReportRow(string ClaimType, long Count, decimal Amount);
public sealed record SchemeReportRow(string SchemeName, string SchemeType, string Status);
