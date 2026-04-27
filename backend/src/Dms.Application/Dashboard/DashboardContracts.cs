namespace Dms.Application.Dashboard;

public sealed record KpiCardDto(string Label, decimal Value, decimal TrendPercent);
public sealed record DashboardResponse(IReadOnlyCollection<KpiCardDto> Kpis, IReadOnlyCollection<ChartPointDto> SalesTrend, IReadOnlyCollection<TopItemDto> TopItems);
public sealed record ChartPointDto(string Label, decimal Value);
public sealed record TopItemDto(string ItemName, decimal Value);
