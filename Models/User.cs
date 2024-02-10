namespace WADProject1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public UserProfile? UserProfile { get; set; }
        public required ICollection<Swipe> SentSwipes { get; set; }
        public required ICollection<Swipe> ReceivedSwipes { get; set; }
        public required ICollection<Match> SentMatches { get; set; }
        public required ICollection<Match> ReceivedMatches { get; set; }
        public required ICollection<Chat> SentMessages { get; set; }
        public required ICollection<Chat> ReceivedMessages { get; set; }
    }
}
