namespace WADProject1.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
    }
}
