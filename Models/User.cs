namespace WADProject1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool IsAdmin { get; set; }
        public UserProfile? UserProfile { get; set; }
        public ICollection<Swipe> SentSwipes { get; set; }
        public ICollection<Swipe> ReceivedSwipes { get; set; }
        public ICollection<Match> SentMatches { get; set; }
        public ICollection<Match> ReceivedMatches { get; set; }
        public ICollection<Chat> SentMessages { get; set; }
        public ICollection<Chat> ReceivedMessages { get; set; }
    }
}
