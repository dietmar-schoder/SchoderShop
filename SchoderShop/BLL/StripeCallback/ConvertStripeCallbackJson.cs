using SchoderChain;
using SchoderShop.Helpers;
using Stripe;
using Stripe.Checkout;

namespace SchoderShop.BLL.StripeCallback
{
    public class ConvertStripeCallbackJson : Processor
    {
        private readonly StripeData _stripeData;
        private readonly IStripeSecrets _stripeSecrets;
        private readonly IStripeEventUtility _stripeEventUtility;

        public ConvertStripeCallbackJson(
            IStripeEventUtility stripeEventUtility,
            StripeData stripeData,
            IStripeSecrets stripeSecrets,
            ISlackManager slackManager)
            : base(slackManager)
        {
            _stripeData = stripeData;
            _stripeSecrets = stripeSecrets;
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
                    _stripeData.HttpRequest.Headers[Constants.HEADER_STRIPE_SIGNATURE],
                    _stripeData.IsTest ? _stripeSecrets.STRIPE_SECRET_TEST : _stripeSecrets.STRIPE_SECRET_LIVE);

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
