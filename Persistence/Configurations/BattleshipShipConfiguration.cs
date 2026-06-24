using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class BattleshipShipConfiguration : IEntityTypeConfiguration<BattleshipShip>
{
    public void Configure(EntityTypeBuilder<BattleshipShip> builder)
    {
        builder.Property(bs => bs.Direction)
            .IsRequired()
            .HasMaxLength(15); 

        builder.Property(bs => bs.PlayerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasOne(bs => bs.Match)
            .WithMany(m => m.Ships)
            .HasForeignKey(bs => bs.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}