using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SchoderChain;
using SchoderShop.Helpers;
using Stripe;
using System;
using System.Threading.Tasks;

namespace SchoderShop.BLL.StripeSession
{
    public class SetStripeParameters : Processor
    {
        private readonly StripeData _stripeData;

        public SetStripeParameters(StripeData stripeData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager) => _stripeData = stripeData;

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998

            StripeConfiguration.ApiKey = _stripeData.IsTest ? StripeSecrets.STRIPE_APIKEY_TEST : StripeSecrets.STRIPE_APIKEY_LIVE;
            var myHomePageUrl = _stripeData.IsTest ? Constants.MYHOMEPAGE_URL_TEST : Constants.MYHOMEPAGE_URL_LIVE;
            _stripeData.CancelUrl = $"{myHomePageUrl}{"Shop/Index"}";
            _stripeData.SuccessUrl = $"{myHomePageUrl}{"Shop/OrderThankYou"}";
            return true;
        }
    }
}
