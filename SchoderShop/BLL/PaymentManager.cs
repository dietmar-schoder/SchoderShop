using SchoderChain;
using SchoderShop.BLL.StripeCallback;
using SchoderShop.BLL.StripeSession;

namespace SchoderShop.BLL
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IChain _chain;

        public PaymentManager(IChain chain, ChainData chainData) => _chain = chain;

        public async Task StartStripeSessionAsync(string calledBy) => await _chain.ProcessAsync(calledBy,
            //typeof(GetDonationProductId),
            typeof(GetProduct),
            typeof(SetStripeParameters),
            typeof(CreateStripeCheckoutSession),
            typeof(InsertStripeSession),
            typeof(RedirectToStripe));

        public async Task ProcessStripeCallbackAsync(string calledBy) => await _chain.ProcessAsync(calledBy,
            typeof(InsertStripeCallback),
            typeof(ConvertStripeCallbackJson),
            typeof(UpdateStripeSession),
            typeof(ProcessStripePayment));
    }
}
