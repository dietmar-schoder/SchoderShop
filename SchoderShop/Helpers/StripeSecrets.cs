namespace SchoderShop.Helpers
{
    public static class StripeSecrets
    {
        public const string HEADER_STRIPE_SIGNATURE = "Stripe-Signature";
        public const string HEADER_STRIPE_REDIRECT = "Location";

        public const string STRIPE_APIKEY_TEST = "sk_test_";
        public const string STRIPE_APIKEY_LIVE = "sk_live_";

        public const string STRIPE_SECRET_TEST = "whsec_";
        public const string STRIPE_SECRET_LIVE = "whsec_";
    }
}
