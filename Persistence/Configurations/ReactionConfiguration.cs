using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpProject.Persistence.Configurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasOne(r => r.Post)
            .WithMany(p => p.Reactions)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}