using Stripe;

namespace SchoderShop.BLL
{
    public class StripeEventUtility : IStripeEventUtility
    {
        public StripeEventUtility() { }

        public Event ConstructEvent(string stripeJson, string StripeSignatureHeader, string secret)
            => EventUtility.ConstructEvent(stripeJson, StripeSignatureHeader, secret, throwOnApiVersionMismatch: false);
    }
}
