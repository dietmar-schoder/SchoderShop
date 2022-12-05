using Stripe;

namespace SchoderShop.BLL
{
    public interface IStripeEventUtility
    {
        Event ConstructEvent(string stripeJson, string StripeSignatureHeader, string secret);
    }
}
