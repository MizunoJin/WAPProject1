using Microsoft.AspNetCore.Identity;

namespace WADProject1.Models
{
    public class User : IdentityUser
    {
        public UserProfile? UserProfile { get; set; }
        public ICollection<Swipe> SentSwipes { get; set; }
        public ICollection<Swipe> ReceivedSwipes { get; set; }
        public ICollection<Match> SentMatches { get; set; }
        public ICollection<Match> ReceivedMatches { get; set; }
        public ICollection<Chat> SentMessages { get; set; }
        public ICollection<Chat> ReceivedMessages { get; set; }
    }
}
