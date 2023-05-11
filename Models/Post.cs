namespace SEWebAppFinal.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public int ParentPostId { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public int PostAuthorId { get; set; }
        public int Score { get; set; }
    }
}
