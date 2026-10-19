namespace PulseOps.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using PulseOps.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<PulseOps.Domain.Entities.Organization> Organizations => Set<PulseOps.Domain.Entities.Organization>();
    public DbSet<PulseOps.Domain.Entities.User> Users => Set<PulseOps.Domain.Entities.User>();
    public DbSet<PulseOps.Domain.Entities.MonitoredService> MonitoredServices => Set<PulseOps.Domain.Entities.MonitoredService>();
    public DbSet<PulseOps.Domain.Entities.MetricLog> MetricLogs => Set<PulseOps.Domain.Entities.MetricLog>();
    public DbSet<PulseOps.Domain.Entities.AlertRule> AlertRules => Set<PulseOps.Domain.Entities.AlertRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}