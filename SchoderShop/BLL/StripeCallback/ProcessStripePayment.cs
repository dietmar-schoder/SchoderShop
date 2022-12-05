using SchoderChain;

namespace SchoderShop.BLL.StripeCallback
{
    public class ProcessStripePayment : Processor
    {
        private readonly StripeData _stripeData;

        public ProcessStripePayment(StripeData stripeData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager) => _stripeData = stripeData;

        protected override async Task<bool> ProcessOkAsync()
        {
            var slackMessage = $"Stripe callback: {_stripeData.StripeEventType}{Environment.NewLine}{_stripeData.StripeJson}";
            if (_stripeData.IsTest)
            {
                await _slackManager.SlackTestMessageAsync(slackMessage);
            }
            else
            {
                await _slackManager.SlackShopMessageAsync(slackMessage);
            }
            //await _slackManager.SlackShopMessageAsync($"PRODUCT SOLD!{Environment.NewLine}" +
            //    $"{parameters.Account?.Email ?? "anonymous"}{Environment.NewLine}" +
            //    $"{parameters.Product.Title}{Environment.NewLine}" +
            //    $"{parameters.Product.PriceFormatted}{Environment.NewLine}" +
            //    $"ProductId: {parameters.Product.Id}{Environment.NewLine}" +
            //    $"Stripe: {parameters.PaymentIntentId}: {parameters.StripeEventType}");
            return true;
        }
    }
}
