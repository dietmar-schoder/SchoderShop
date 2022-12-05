using SchoderChain;
using SchoderShop.DAL;
using SchoderShop.Helpers;

namespace SchoderShop.BLL.StripeSession
{
    public class InsertStripeSession : Processor
    {
        private readonly IStripeSessionAccessor _stripeSessionAccessor;
        private readonly IDateTimeFactory _dateTimeFactory;
        private readonly StripeData _stripeData;

        public InsertStripeSession(
            IStripeSessionAccessor stripeSessionAccessor,
            IDateTimeFactory dateTimeFactory,
            StripeData stripeData,
            ChainData chainData,
            ISlackManager slackManager)
            : base(chainData, slackManager)
        {
            _stripeSessionAccessor = stripeSessionAccessor;
            _dateTimeFactory = dateTimeFactory;
            _stripeData = stripeData;
        }

        protected override async Task<bool> ProcessOkAsync()
        {
            var now = _dateTimeFactory.UtcNow;

            await _stripeSessionAccessor.InsertAsync(
                Id: _stripeData.StripeCheckoutSession.Id,
                ProductId: _stripeData.Product?.Id,
                PriceAsInteger: _stripeData.Product.PriceAsInteger,
                Currency: _stripeData.Product?.Currency,
                ExpiresAtDateTime: _stripeData.StripeCheckoutSession.ExpiresAt,
                CreatedDateTime: now,
                UpdatedDateTime: now);

            return true;
        }
    }
}
