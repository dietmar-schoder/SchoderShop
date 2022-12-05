using SchoderChain;
using SchoderShop.DAL;

namespace SchoderShop.BLL.StripeCallback
{
    public class InsertStripeCallback : Processor
    {
        private readonly IStripeCallbackAccessor _stripeCallbackAccessor;
        private readonly StripeData _stripeData;

        public InsertStripeCallback(IStripeCallbackAccessor stripeCallbackAccessor, StripeData stripeData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager)
        {
            _stripeCallbackAccessor = stripeCallbackAccessor;
            _stripeData = stripeData;
        }

        protected override async Task<bool> ProcessOkAsync()
        {
            _stripeData.StripeJson = await new StreamReader(_stripeData.HttpRequest.Body).ReadToEndAsync();
            await _stripeCallbackAccessor.InsertAsync(_stripeData.StripeJson);
            return true;
        }
    }
}
