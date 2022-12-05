namespace SchoderShop.Models
{
    public interface IStripeSession
    {
        string Id { get; set; }

        Guid? ProductId { get; set; }

        int PriceAsInteger { get; set; }

        string Currency { get; set; }

        string LastEventType { get; set; }

        DateTime ExpiresAtDateTime { get; set; }

        DateTime CreatedDateTime { get; set; }

        DateTime UpdatedDateTime { get; set; }
    }
}
