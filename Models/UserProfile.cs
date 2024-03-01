namespace WADProject1.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public required string Name { get; set; }
        public required string Detail { get; set; }
        public required string SexualOrientation { get; set; }
        public required string UserId { get; set; }
        public User User { get; set; }
    }
}
