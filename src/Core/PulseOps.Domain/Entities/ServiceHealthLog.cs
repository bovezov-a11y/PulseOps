namespace PulseOps.Domain.Entities;

public class ServiceHealthLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MonitoredServiceId { get; set; }
    public MonitoredService? MonitoredService { get; set; }

    public int StatusCode { get; set; }
    public bool IsHealthy { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? ErrorMessage { get; set; }

    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}