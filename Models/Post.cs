namespace ABABI.Models
{
    public class Post
    {
        public Guid PostId { get; set; }
        public Guid ParentPostId { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public long PostAuthorId { get; set; }
        public long Score { get; set; }
    }
}
