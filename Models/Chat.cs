namespace WADProject1.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public required User Sender { get; set; }
        public int ReceiverId { get; set; }
        public required User Receiver { get; set; }
        public required string Content { get; set; }
    }
}
