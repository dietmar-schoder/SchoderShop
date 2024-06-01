using SchoderChain;
using SchoderShop.Helpers;
using Stripe;

namespace SchoderShop.BLL.StripeSession
{
    public class SetStripeParameters : Processor
    {
        private readonly StripeData _stripeData;
        private readonly IStripeSecrets _stripeSecrets;

        public SetStripeParameters(StripeData stripeData, IStripeSecrets stripeSecrets, ISlackManager slackManager) : base(slackManager)
        {
            _stripeData = stripeData;
            _stripeSecrets = stripeSecrets;
        }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998

            StripeConfiguration.ApiKey = _stripeData.IsTest ? _stripeSecrets.STRIPE_APIKEY_TEST : _stripeSecrets.STRIPE_APIKEY_LIVE;

            return true;
        }
    }
}
