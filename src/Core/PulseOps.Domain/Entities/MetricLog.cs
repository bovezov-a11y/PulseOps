namespace PulseOps.Domain.Entities;

using PulseOps.Domain.Common;

public class MetricLog : BaseEntity
{
    public Guid MonitoredServiceId { get; set; }
    public MonitoredService MonitoredService { get; set; } = default!;

    public int StatusCode { get; set; }
    public double ResponseTimeMs { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}