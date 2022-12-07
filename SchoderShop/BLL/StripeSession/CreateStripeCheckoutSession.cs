using SchoderChain;
using SchoderShop.Helpers;
using Stripe.Checkout;

namespace SchoderShop.BLL.StripeSession
{
    public class CreateStripeCheckoutSession : Processor
    {
        private readonly IDateTimeFactory _dateTimeFactory;
        private readonly StripeData _stripeData;

        public CreateStripeCheckoutSession(IDateTimeFactory dateTimeFactory, StripeData stripeData, ChainData chainData, ISlackManager slackManager)
            : base(chainData, slackManager)
        {
            _dateTimeFactory = dateTimeFactory;
            _stripeData = stripeData;
        }

#pragma warning disable 1998
        protected override async Task<bool> ProcessOkAsync()
        {
#pragma warning restore CS1998
            _stripeData.StripeCheckoutSession = new SessionService().Create(GetSessionCreateOptions());
            return _stripeData.StripeCheckoutSession is not null;

            SessionCreateOptions GetSessionCreateOptions()
            {
                return GetStripeSessionCreateOptions
                (
                    customerEmail: _stripeData.Email,
                    productId: _stripeData.Product.Id,
                    name: _stripeData.Product.Title,
                    description: _stripeData.Product.ShortDescription ?? _stripeData.Product.Title,
                    imageFileUrl: _stripeData.Product.ImageFileUrl,
                    priceAsInteger: _stripeData.Product.PriceAsInteger,
                    currency: _stripeData.Currency,
                    quantity: _stripeData.Quantity,
                    dateTime: _dateTimeFactory.UtcNow,
                    cancelUrl: _stripeData.CancelUrl,
                    successUrl: _stripeData.SuccessUrl
                );
            }

            SessionCreateOptions GetStripeSessionCreateOptions(
                long? quantity,
                string name,
                string description,
                Guid productId,
                string imageFileUrl,
                long? priceAsInteger,
                string currency,
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
                                    Images = new List<string> { imageFileUrl }
                                },
                                UnitAmount = priceAsInteger,
                                Currency = currency // "gbp"
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
