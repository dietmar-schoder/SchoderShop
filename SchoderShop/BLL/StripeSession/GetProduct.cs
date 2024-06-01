using SchoderChain;
using SchoderShop.DAL;

namespace SchoderShop.BLL.StripeSession
{
    public class GetProduct : Processor
    {
        private readonly IProductAccessor _productAccessor;
        private readonly StripeData _stripeData;

        public GetProduct(IProductAccessor productAccessor, StripeData stripeData, ISlackManager slackManager) : base(slackManager)
        {
            _productAccessor = productAccessor;
            _stripeData = stripeData;
        }

        protected override async Task<bool> ProcessOkAsync()
        {
            _stripeData.Product = await _productAccessor.GetAsync(_stripeData.ProductId);
            return _stripeData.Product is not null;
        }
    }
}
