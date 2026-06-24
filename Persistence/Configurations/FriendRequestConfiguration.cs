using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.Property(fr => fr.Status)
            .IsRequired()
            .HasMaxLength(30);
    }
}