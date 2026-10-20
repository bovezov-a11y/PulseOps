namespace PulseOps.Domain.Entities;

public class MonitoredService
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int CheckIntervalSeconds { get; set; } = 60;
    public bool IsActive { get; set; } = true;

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public ICollection<ServiceHealthLog> HealthLogs { get; set; } = new List<ServiceHealthLog>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}