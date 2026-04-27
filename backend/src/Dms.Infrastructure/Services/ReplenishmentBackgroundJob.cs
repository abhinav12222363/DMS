using Dms.Application.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dms.Infrastructure.Services;

public sealed class ReplenishmentBackgroundJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReplenishmentBackgroundJob> _logger;

    public ReplenishmentBackgroundJob(IServiceScopeFactory scopeFactory, ILogger<ReplenishmentBackgroundJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var svc = scope.ServiceProvider.GetRequiredService<IReplenishmentService>();
                await svc.RunAutoSuggestAsync(stoppingToken);
                _logger.LogInformation("Replenishment auto suggest completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Replenishment background job failed");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
