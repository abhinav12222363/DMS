using Dms.Application.Abstractions;
using Dms.Application.Common;
using Dms.Application.Transactions;
using Dms.Domain.Entities;

namespace Dms.Infrastructure.Services;

public sealed class SalesOrderService : ISalesOrderService
{
    private readonly ISalesOrderRepository _salesOrderRepository;

    public SalesOrderService(ISalesOrderRepository salesOrderRepository) => _salesOrderRepository = salesOrderRepository;

    public async Task<PagedResult<SalesOrderDto>> GetAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var paged = await _salesOrderRepository.GetPagedAsync(distributorId, pageNumber, pageSize, ct);
        return new PagedResult<SalesOrderDto>(
            paged.Items.Select(x => new SalesOrderDto(x.Id, x.OrderNumber, x.OrderDate, x.TotalAmount, x.Status, x.DistributorId)).ToArray(),
            pageNumber,
            pageSize,
            paged.TotalCount);
    }

    public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderRequest request, CancellationToken ct)
    {
        var entity = new SalesOrder
        {
            OrderNumber = request.OrderNumber,
            OrderDate = request.OrderDate,
            TotalAmount = request.TotalAmount,
            DistributorId = request.DistributorId,
            Status = "Submitted"
        };

        await _salesOrderRepository.AddAsync(entity, ct);
        await _salesOrderRepository.SaveChangesAsync(ct);
        return new SalesOrderDto(entity.Id, entity.OrderNumber, entity.OrderDate, entity.TotalAmount, entity.Status, entity.DistributorId);
    }
}
