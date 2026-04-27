using Dms.Application.Common;

namespace Dms.Application.Transactions;

public sealed record SalesOrderDto(Guid Id, string OrderNumber, DateOnly OrderDate, decimal TotalAmount, string Status, Guid DistributorId);
public sealed record CreateSalesOrderRequest(string OrderNumber, DateOnly OrderDate, decimal TotalAmount, Guid DistributorId);

public interface ISalesOrderService
{
    Task<PagedResult<SalesOrderDto>> GetAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct);
    Task<SalesOrderDto> CreateAsync(CreateSalesOrderRequest request, CancellationToken ct);
}
