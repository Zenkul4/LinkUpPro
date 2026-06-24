using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class BattleshipMatchConfiguration : IEntityTypeConfiguration<BattleshipMatch>
{
    public void Configure(EntityTypeBuilder<BattleshipMatch> builder)
    {
        builder.Property(bm => bm.Status)
            .IsRequired()
            .HasMaxLength(30); 

        builder.Property(bm => bm.CurrentTurnId)
            .HasMaxLength(450);

        builder.Property(bm => bm.WinnerId)
            .HasMaxLength(450);
    }
}