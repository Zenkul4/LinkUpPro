using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.ActivityType)
            .IsRequired()
            .HasMaxLength(50); 

        builder.Property(n => n.Description)
            .IsRequired()
            .HasMaxLength(255);
    }
}