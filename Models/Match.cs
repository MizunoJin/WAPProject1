using System.Text.Json.Serialization;

namespace WADProject1.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public string SenderId { get; set; }
        [JsonIgnore]
        public User Sender { get; set; }
        public string ReceiverId { get; set; }
        [JsonIgnore]
        public User Receiver { get; set; }
    }
}
