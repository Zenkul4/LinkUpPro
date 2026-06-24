using LinkUpProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpProject.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);

        builder.HasMany(u => u.Posts)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Comments)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Reactions)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.SentRequests)
            .WithOne(fr => fr.Sender)
            .HasForeignKey(fr => fr.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReceivedRequests)
            .WithOne(fr => fr.Receiver)
            .HasForeignKey(fr => fr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.FriendshipsInitiated)
            .WithOne(f => f.User1)
            .HasForeignKey(f => f.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.FriendshipsReceived)
            .WithOne(f => f.User2)
            .HasForeignKey(f => f.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.SentNotifications)
            .WithOne(n => n.Sender)
            .HasForeignKey(n => n.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReceivedNotifications)
            .WithOne(n => n.Receiver)
            .HasForeignKey(n => n.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.MatchesAsPlayer1)
            .WithOne(bm => bm.Player1)
            .HasForeignKey(bm => bm.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.MatchesAsPlayer2)
            .WithOne(bm => bm.Player2)
            .HasForeignKey(bm => bm.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}