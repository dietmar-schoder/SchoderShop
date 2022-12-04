using SchoderChain;
using SchoderShop.Helpers;
using Stripe.Checkout;

namespace SchoderShop.BLL
{
    public class CreateStripeCheckoutSession : Processor
    {
        private readonly IDateTimeFactory _dateTimeFactory;
        private readonly ShopData _shopData;

        public CreateStripeCheckoutSession(IDateTimeFactory dateTimeFactory, ShopData shopData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager)
        {
            _dateTimeFactory = dateTimeFactory;
            _shopData = shopData;
        }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998
            _shopData.StripeCheckoutSession = new SessionService().Create(GetSessionCreateOptions());
            return _shopData.StripeCheckoutSession is not null;

            SessionCreateOptions GetSessionCreateOptions()
            {
                return GetStripeSessionCreateOptions
                (
                    customerEmail: _shopData.Account?.Email ?? _shopData.Email,
                    productId: _shopData.Product.Id,
                    name: _shopData.Product.Title,
                    description: _shopData.Product.ShortDescription ?? _shopData.Product.Title,
                    imageFile: _shopData.Product.ImgFileName800Jpg,
                    priceInPence: _shopData.Product.PriceInPence,
                    quantity: 1,
                    dateTime: _dateTimeFactory.UtcNow,
                    schoderUrl: Constants.MYHOMEPAGE_URL_LIVE,
                    cancelUrl: _shopData.CancelUrl,
                    successUrl: _shopData.SuccessUrl
                );
            }

            SessionCreateOptions GetStripeSessionCreateOptions(
                long? quantity,
                string name,
                string description,
                Guid productId,
                string schoderUrl,
                string imageFile,
                long? priceInPence,
                string cancelUrl,
                string successUrl,
                string customerEmail,
                DateTime dateTime)
            {
                return new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Quantity = quantity,
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = name,
                                    Description = description,
                                    Metadata = new Dictionary<string, string> { { "product_id", productId.ToString() } },
                                    Images = new List<string> { $@"{schoderUrl}img/{imageFile}" }
                                },
                                UnitAmount = priceInPence,
                                Currency = "gbp"
                            }
                        }
                    },
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    CancelUrl = cancelUrl,
                    SuccessUrl = successUrl,
                    CustomerEmail = customerEmail,
                    ExpiresAt = dateTime.AddMinutes(30) // 60
                };
            }
        }
    }
}
