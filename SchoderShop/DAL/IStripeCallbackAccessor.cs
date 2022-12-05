using System.Threading.Tasks;

namespace SchoderShop.DAL
{
    public interface IStripeCallbackAccessor
    {
        Task InsertAsync(string json);
    }
}
