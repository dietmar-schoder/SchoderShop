using SchoderShop.Models;

namespace SchoderShopUnitTests.TestModels
{
    public class TestStripeSession : IStripeSession
    {
        public string Id { get; set; }
        public Guid? ProductId { get; set; }
        public int PriceAsInteger { get; set; }
        public string Currency { get; set; }
        public string LastEventType { get; set; }
        public DateTime ExpiresAtDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}
