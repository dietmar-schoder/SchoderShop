using SchoderShop.Models;

namespace SchoderShop.DAL
{
    public interface IProductAccessor
    {
        Task<IProduct> GetAsync(Guid id);
    }
}
