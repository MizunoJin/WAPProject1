namespace WADProject1.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string? SenderId { get; set; }
        public User? Sender { get; set; }
        public string? ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public required string Content { get; set; }
    }
}
