using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace LinkUpProject.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
    public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
    public ICollection<FriendRequest> SentRequests { get; set; } = new List<FriendRequest>();
    public ICollection<FriendRequest> ReceivedRequests { get; set; } = new List<FriendRequest>();
    public ICollection<Friendship> FriendshipsInitiated { get; set; } = new List<Friendship>();
    public ICollection<Friendship> FriendshipsReceived { get; set; } = new List<Friendship>();
    public ICollection<BattleshipMatch> MatchesAsPlayer1 { get; set; } = new List<BattleshipMatch>();
    public ICollection<BattleshipMatch> MatchesAsPlayer2 { get; set; } = new List<BattleshipMatch>();
}