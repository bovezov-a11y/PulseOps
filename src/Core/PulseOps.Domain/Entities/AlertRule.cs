namespace PulseOps.Domain.Entities;

using PulseOps.Domain.Common;

public class AlertRule : BaseEntity
{
    public Guid MonitoredServiceId { get; set; }
    public MonitoredService MonitoredService { get; set; } = default!;

    public string RuleName { get; set; } = default!;
    public int ThresholdResponseTimeMs { get; set; } = 2000;
    public int ConsecutiveFailuresCount { get; set; } = 3;
    public string NotificationChannel { get; set; } = "Email";
    public string Destination { get; set; } = default!;
    public bool IsEnabled { get; set; } = true;
}