namespace SchoderShop.Helpers
{
    public interface IStripeSecrets
    {
        string STRIPE_APIKEY_TEST { get; } // "sk_test_";

        string STRIPE_APIKEY_LIVE { get; } // "sk_live_";

        string STRIPE_SECRET_TEST { get; } //  "whsec_TEST";

        string STRIPE_SECRET_LIVE { get; } //  "whsec_LIVE";
    }
}
