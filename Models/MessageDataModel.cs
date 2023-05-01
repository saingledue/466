namespace SEWebApp.Models
{
    public class MessageDataModel
    {
        public long Id { get; set; }

        public long SenderId { get; set; }

        public long RecipientId { get; set; }

        public byte[] Content { get; set; }

        public int Emoji { get; set; }
        public DateTime Time { get; set; }
        public bool Read { get; set; }
    }
}
