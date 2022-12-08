using Microsoft.AspNetCore.Mvc;
using SchoderChain;
using SchoderShop.Helpers;

namespace SchoderShop.BLL.StripeSession
{
    public class RedirectToStripe : Processor
    {
        private readonly StripeData _stripeData;

        public RedirectToStripe(StripeData stripeData, ISlackManager slackManager)
            : base(slackManager) => _stripeData = stripeData;

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998
            _stripeData.HttpResponse.Headers.Add(StripeSecrets.HEADER_STRIPE_REDIRECT, _stripeData.StripeCheckoutSession.Url);
            _stripeData.ActionResult = new StatusCodeResult(303);
            return true;
        }
    }
}
