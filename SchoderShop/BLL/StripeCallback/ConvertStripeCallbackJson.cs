using SchoderChain;
using SchoderShop.Helpers;
using Stripe;
using Stripe.Checkout;

namespace SchoderShop.BLL.StripeCallback
{
    public class ConvertStripeCallbackJson : Processor
    {
        private readonly StripeData _stripeData;
        private readonly IStripeEventUtility _stripeEventUtility;

        public ConvertStripeCallbackJson(IStripeEventUtility stripeEventUtility, StripeData stripeData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager)
        {
            _stripeData = stripeData;
            _stripeEventUtility = stripeEventUtility;
        }

#pragma warning disable CS1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998
            try
            {
                var stripeEvent = _stripeEventUtility.ConstructEvent(
                    _stripeData.StripeJson,
                    _stripeData.HttpRequest.Headers[StripeSecrets.HEADER_STRIPE_SIGNATURE],
                    _stripeData.IsTest ? StripeSecrets.STRIPE_SECRET_TEST : StripeSecrets.STRIPE_SECRET_LIVE);

                _stripeData.StripeEventType = stripeEvent.Type;
                _stripeData.StripeCheckoutSession = stripeEvent.Data.Object as Session;
            }
            catch (StripeException ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
    }
}
