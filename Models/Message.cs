namespace SEWebApp.Models
{
    public class Message
    {
        public long Id { get; set; }

        public long SenderId { get; set; }

        public long RecipientId { get; set; }

        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool Read { get; set; }
    }
}
