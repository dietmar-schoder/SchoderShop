namespace SchoderShop.Models
{
    public interface IProduct
    {
        Guid Id { get; set; }

        string Title { get; set; }

        string ShortDescription { get; set; }

        string ImageFileUrl { get; set; }

        int PriceAsInteger { get; set; }

        string Currency { get; set; }
    }
}
