using Dms.Application.Auth;
using Dms.Application.Common;
using Dms.Application.Dashboard;
using Dms.Application.Distributors;
using Dms.Application.Reports;
using Dms.Application.Transactions;
using Dms.Domain.Entities;

namespace Dms.Application.Abstractions;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
}

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, DateTime expiresAtUtc);
}

public interface ICaptchaValidator
{
    Task<bool> ValidateAsync(string token, CancellationToken ct);
}

public interface IPasswordResetTokenStore
{
    Task StoreAsync(Guid userId, string token, CancellationToken ct);
    Task<Guid?> ConsumeAsync(string token, CancellationToken ct);
}

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IDistributorRepository
{
    Task<IReadOnlyCollection<DistributorDto>> GetForUserAsync(Guid userId, CancellationToken ct);
}

public interface IItemRepository
{
    Task<PagedResult<Item>> GetPagedAsync(string? search, int pageNumber, int pageSize, CancellationToken ct);
    Task AddAsync(Item item, CancellationToken ct);
    Task<Item?> GetByIdAsync(Guid id, CancellationToken ct);
    Task RemoveAsync(Item item, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface ISalesOrderRepository
{
    Task<PagedResult<SalesOrder>> GetPagedAsync(Guid distributorId, int pageNumber, int pageSize, CancellationToken ct);
    Task AddAsync(SalesOrder order, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<DashboardResponse> GetDashboardAsync(Guid distributorId, DateOnly fromDate, DateOnly toDate, CancellationToken ct);
    Task<IReadOnlyCollection<SalesReportRow>> GetSalesReportAsync(ReportFilter filter, CancellationToken ct, bool readOnlyReplica = false);
}

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct);
}

public interface IWorkflowRepository
{
    Task<Dms.Domain.Entities.Distributor?> GetDistributorAsync(Guid distributorId, CancellationToken ct);
    Task<Dms.Domain.Entities.Order?> GetOrderAsync(Guid orderId, CancellationToken ct);
    Task<Dms.Domain.Entities.Item?> GetItemAsync(Guid itemId, CancellationToken ct);
    Task<Dms.Domain.Entities.Item?> GetItemByCodeAsync(string itemCode, CancellationToken ct);
    Task<Dms.Domain.Entities.Distributor?> GetDistributorByCodeAsync(string code, CancellationToken ct);
    Task AddItemAsync(Dms.Domain.Entities.Item item, CancellationToken ct);
    Task AddDistributorAsync(Dms.Domain.Entities.Distributor distributor, CancellationToken ct);
    Task AddStockAsync(Dms.Domain.Entities.Stock stock, CancellationToken ct);
    Task<Dms.Domain.Entities.Stock?> GetStockAsync(Guid distributorId, Guid itemId, CancellationToken ct);
    Task<Dms.Domain.Entities.Scheme?> GetSchemeAsync(Guid schemeId, CancellationToken ct);
    Task<Dms.Domain.Entities.Claim?> GetClaimAsync(Guid claimId, CancellationToken ct);
    IQueryable<Dms.Domain.Entities.Order> OrdersQuery();
    IQueryable<Dms.Domain.Entities.Scheme> SchemesQuery();
    IQueryable<Dms.Domain.Entities.Claim> ClaimsQuery();
    IQueryable<Dms.Domain.Entities.Stock> StocksQuery();
    Task AddOrderAsync(Dms.Domain.Entities.Order order, CancellationToken ct);
    Task AddInvoiceAsync(Dms.Domain.Entities.Invoice invoice, CancellationToken ct);
    Task AddSchemeAsync(Dms.Domain.Entities.Scheme scheme, CancellationToken ct);
    Task AddClaimAsync(Dms.Domain.Entities.Claim claim, CancellationToken ct);
    Task AddReplenishmentAsync(Dms.Domain.Entities.Replenishment replenishment, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
