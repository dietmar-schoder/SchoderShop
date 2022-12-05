namespace SchoderShop.BLL
{
    public interface IPaymentManager
    {
        Task StartStripeSessionAsync(string caller);

        Task ProcessStripeCallbackAsync(string caller);
    }
}
