namespace PulseOps.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseOps.Domain.Entities;

public class MonitoredServiceConfiguration : IEntityTypeConfiguration<MonitoredService>
{
    public void Configure(EntityTypeBuilder<MonitoredService> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.Organization)
            .WithMany(x => x.MonitoredServices)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}