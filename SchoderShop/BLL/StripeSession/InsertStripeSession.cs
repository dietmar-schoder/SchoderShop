using SchoderChain;
using SchoderShop.DAL;

namespace SchoderShop.BLL.StripeSession
{
    public class InsertStripeSession : Processor
    {
        private readonly IStripeSessionAccessor _stripeSessionAccessor;
        private readonly StripeData _stripeData;

        public InsertStripeSession(IStripeSessionAccessor stripeSessionAccessor, StripeData stripeData, ISlackManager slackManager) : base(slackManager)
        {
            _stripeSessionAccessor = stripeSessionAccessor;
            _stripeData = stripeData;
        }

        protected override async Task<bool> ProcessOkAsync()
        {
            await _stripeSessionAccessor.InsertAsync(
                Id: _stripeData.StripeCheckoutSession.Id,
                ProductId: _stripeData.Product?.Id,
                PriceAsInteger: _stripeData.Product.PriceAsInteger,
                Currency: _stripeData.Product?.Currency,
                ExpiresAtDateTime: _stripeData.StripeCheckoutSession.ExpiresAt,
                CreatedDateTime: _stripeData.CreatedDateTime,
                UpdatedDateTime: _stripeData.CreatedDateTime);

            return true;
        }
    }
}
