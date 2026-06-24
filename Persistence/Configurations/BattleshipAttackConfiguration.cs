using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpProject.Persistence.Configurations;

public class BattleshipAttackConfiguration : IEntityTypeConfiguration<BattleshipAttack>
{
    public void Configure(EntityTypeBuilder<BattleshipAttack> builder)
    {
        builder.HasOne(ba => ba.Match)
            .WithMany(m => m.Attacks)
            .HasForeignKey(ba => ba.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}