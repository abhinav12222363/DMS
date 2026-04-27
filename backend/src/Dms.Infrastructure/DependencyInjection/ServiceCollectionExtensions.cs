using System.Text;
using Dms.Application.Abstractions;
using Dms.Application.Masters;
using Dms.Application.Transactions;
using Dms.Infrastructure.Auth;
using Dms.Infrastructure.Services;
using Dms.Persistence;
using Dms.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Dms.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDmsInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var writeConnection = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection missing");
        var readConnection = config.GetConnectionString("ReadReplicaConnection") ?? writeConnection;

        services.AddDbContext<DmsDbContext>(opt => opt.UseNpgsql(writeConnection));
        services.AddSingleton(provider =>
        {
            var builder = new DbContextOptionsBuilder<DmsDbContext>();
            builder.UseNpgsql(readConnection);
            return builder.Options;
        });

        services.AddScoped<IReadOnlyDmsDbContextFactory, ReadOnlyDmsDbContextFactory>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDistributorRepository, DistributorRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ISalesOrderService, SalesOrderService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IDistributorService, DistributorService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICaptchaValidator, CaptchaValidator>();
        services.AddScoped<IPasswordResetTokenStore, PasswordResetTokenStore>();
        services.AddScoped<IEmailSender, EmailSender>();

        services.AddDistributedMemoryCache(); // Swap to Redis in production

        var signingKey = config["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt signing key is missing");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
            });

        services.AddAuthorization();
        return services;
    }
}
