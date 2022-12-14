using SchoderChain;
using SchoderShop.DAL;

namespace SchoderShop.BLL.StripeCallback
{
    public class UpdateStripeSession : Processor
    {
        private readonly IStripeSessionAccessor _stripeSessionAccessor;
        private readonly StripeData _stripeData;

        public UpdateStripeSession(IStripeSessionAccessor stripeSessionAccessor, StripeData stripeData, ISlackManager slackManager)
            : base(slackManager)
        {
            _stripeSessionAccessor = stripeSessionAccessor;
            _stripeData = stripeData;
        }

        protected override async Task<bool> ProcessOkAsync()
        {
            await _stripeSessionAccessor.UpdateEventTypeAsync(_stripeData.StripeCheckoutSession.Id, _stripeData.StripeEventType);
            return true;
        }
    }
}
