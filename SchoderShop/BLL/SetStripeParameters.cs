using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SchoderChain;
using SchoderShop.Helpers;
using Stripe;
using System;
using System.Threading.Tasks;

namespace SchoderShop.BLL
{
    public class SetStripeParameters : Processor
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ShopData _shopData;

        public SetStripeParameters(IWebHostEnvironment webHostEnvironment, ShopData shopData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _shopData = shopData;
        }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998

            StripeConfiguration.ApiKey = _webHostEnvironment.IsDevelopment() ? StripeSecrets.STRIPE_APIKEY_TEST : StripeSecrets.STRIPE_APIKEY_LIVE;
            var myHomePageUrl = _webHostEnvironment.IsDevelopment() ? Constants.MYHOMEPAGE_URL_TEST : Constants.MYHOMEPAGE_URL_LIVE;
            _shopData.CancelUrl = $"{myHomePageUrl}{"Shop/Index"}";
            _shopData.SuccessUrl = $"{myHomePageUrl}{"Shop/OrderThankYou"}";
            return true;
        }
    }
}
