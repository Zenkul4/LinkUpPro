using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.ContentType)
            .IsRequired()
            .HasMaxLength(30); 

        builder.Property(p => p.MediaUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Privacy)
            .IsRequired()
            .HasMaxLength(20);
    }
}