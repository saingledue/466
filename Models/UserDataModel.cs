namespace ABABI.Models
{
    public class UserDataModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public byte[] Password { get; set; }

        public byte[] Email { get; set; }

        public int AvatarId { get; set; }

        public DateTime LastLoginTime { get; set; }
        public bool WhiteList { get; set; }
    }
}
