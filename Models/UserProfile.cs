namespace WADProject1.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public required string Name { get; set; }
        public required string Detail { get; set; }
        public required string SexualOrientation { get; set; }
        public required int UserId { get; set; }
        public required User User { get; set; }
    }
}
