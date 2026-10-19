namespace PulseOps.Domain.Entities;

using PulseOps.Domain.Common;
using PulseOps.Domain.Enums;

public class MonitoredService : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Url { get; set; } = default!;
    public CheckType CheckType { get; set; } = CheckType.Https;
    public int IntervalInSeconds { get; set; } = 30;
    public int TimeoutInMilliseconds { get; set; } = 5000;
    public bool IsActive { get; set; } = true;
    public ServiceStatus LastStatus { get; set; } = ServiceStatus.Unknown;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;

    public ICollection<MetricLog> MetricLogs { get; set; } = new List<MetricLog>();
    public ICollection<AlertRule> AlertRules { get; set; } = new List<AlertRule>();
}