namespace SEWebApp.Models
{
    public class Purchase
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long StickerId { get; set; }

        public DateTime PurchaseTime { get; set; }

    }
}
