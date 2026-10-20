using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PulseOps.Domain.Entities;
using PulseOps.Infrastructure.Persistence;

namespace PulseOps.Infrastructure.Services;

public class MonitoringBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MonitoringBackgroundService> _logger;
    private readonly HttpClient _httpClient;

    public MonitoringBackgroundService(IServiceProvider serviceProvider, ILogger<MonitoringBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PulseOps Monitoring Service başlatıldı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var activeServices = await dbContext.MonitoredServices
                    .Where(s => s.IsActive)
                    .ToListAsync(stoppingToken);

                foreach (var service in activeServices)
                {
                    await CheckServiceHealthAsync(dbContext, service, stoppingToken);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Monitoring döngüsünde hata oluştu.");
            }

            // 15 saniyede bir tarama yap
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }

    private async Task CheckServiceHealthAsync(ApplicationDbContext dbContext, MonitoredService service, CancellationToken stoppingToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var healthLog = new ServiceHealthLog
        {
            MonitoredServiceId = service.Id,
            CheckedAt = DateTime.UtcNow
        };

        try
        {
            var response = await _httpClient.GetAsync(service.Url, stoppingToken);
            stopwatch.Stop();

            healthLog.StatusCode = (int)response.StatusCode;
            healthLog.IsHealthy = response.IsSuccessStatusCode;
            healthLog.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            healthLog.StatusCode = 0;
            healthLog.IsHealthy = false;
            healthLog.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            healthLog.ErrorMessage = ex.Message;
        }

        dbContext.ServiceHealthLogs.Add(healthLog);
        _logger.LogInformation($"[{service.Name}] -> Status: {healthLog.StatusCode}, Latency: {healthLog.ResponseTimeMs}ms, Healthy: {healthLog.IsHealthy}");
    }
}