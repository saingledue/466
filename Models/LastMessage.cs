namespace ABABI.Models
{
    public class LastMessage
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string lastMessage { get; set; }
        public bool Read { get; set; }
    }
}
