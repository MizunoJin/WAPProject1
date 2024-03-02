using System.Text.Json.Serialization;

namespace WADProject1.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string? SenderId { get; set; }
        [JsonIgnore]
        public User? Sender { get; set; }
        public string? ReceiverId { get; set; }
        [JsonIgnore]
        public User? Receiver { get; set; }
        public required string Content { get; set; }
    }
}
