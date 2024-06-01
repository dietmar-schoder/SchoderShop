using SchoderShop.Helpers;

namespace SchoderShopUnitTests.TestHelpers
{
    public class TestStripeSecrets : IStripeSecrets
    {
        public string STRIPE_APIKEY_TEST => "sk_test_";

        public string STRIPE_APIKEY_LIVE => "sk_live_";

        public string STRIPE_SECRET_TEST => "whsec_TEST";

        public string STRIPE_SECRET_LIVE => "whsec_LIVE";
    }
}
