namespace PulseOps.Domain.Entities;

using PulseOps.Domain.Common;

public class Organization : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<MonitoredService> MonitoredServices { get; set; } = new List<MonitoredService>();
}