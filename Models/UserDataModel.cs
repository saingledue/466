namespace SEWebApp.Models
{
    public class UserDataModel
    {
        public long Id { get; set; }

        public byte[] Name { get; set; }

        public string Username { get; set; }

        public byte[] Password { get; set; }

        public byte[] Email { get; set; }

        public int AvatarId { get; set; }

        public int PrivacySetting { get; set; }

        public int SpendablePoints { get; set; }

        public int GiftablePoints { get; set; }

        public int TotalPoints { get; set; }

        public DateTime LastLoginTime { get; set; }
        public DateTime LastMessageSent { get; set; }
    }
}
