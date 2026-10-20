using Microsoft.EntityFrameworkCore;
using PulseOps.Domain.Entities;

namespace PulseOps.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<MonitoredService> MonitoredServices => Set<MonitoredService>();
    public DbSet<ServiceHealthLog> ServiceHealthLogs => Set<ServiceHealthLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MonitoredService - Organization ilişkisi
        modelBuilder.Entity<MonitoredService>()
            .HasOne(m => m.Organization)
            .WithMany()
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ServiceHealthLog - MonitoredService ilişkisi
        modelBuilder.Entity<ServiceHealthLog>()
            .HasOne(l => l.MonitoredService)
            .WithMany(m => m.HealthLogs)
            .HasForeignKey(l => l.MonitoredServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}