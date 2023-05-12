namespace ABABI.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public int AvatarId { get; set; }

        public DateTime LastLoginTime { get; set; }
        public bool WhiteList { get; set; }

    }
}
