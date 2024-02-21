namespace WADProject1.Models
{
    public class Swipe
    {
        public int SwipeId { get; set; }
        public string SenderId { get; set; }
        public User Sender { get; set; }
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }
    }
}
