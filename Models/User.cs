using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace WADProject1.Models
{
    public class User : IdentityUser
    {
        public UserProfile? UserProfile { get; set; }
        [JsonIgnore]
        public ICollection<Swipe>? SentSwipes { get; set; }
        [JsonIgnore]
        public ICollection<Swipe>? ReceivedSwipes { get; set; }
        [JsonIgnore]
        public ICollection<Match>? SentMatches { get; set; }
        [JsonIgnore]
        public ICollection<Match>? ReceivedMatches { get; set; }
        [JsonIgnore]
        public ICollection<Chat>? SentMessages { get; set; }
        [JsonIgnore]
        public ICollection<Chat>? ReceivedMessages { get; set; }
    }
}
