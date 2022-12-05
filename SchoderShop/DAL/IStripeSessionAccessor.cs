using SchoderShop.Models;
using System.Threading.Tasks;

namespace SchoderShop.DAL
{
    public interface IStripeSessionAccessor
    {
        Task<IStripeSession> GetAsync(string id);

        Task InsertAsync(
            string Id,
            Guid? ProductId,
            int PriceAsInteger,
            string Currency,
            DateTime ExpiresAtDateTime,
            DateTime CreatedDateTime,
            DateTime UpdatedDateTime);

        Task UpdateEventTypeAsync(string id, string latestEventType);
    }
}
