using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoderShop.Models;
using Stripe.Checkout;

namespace SchoderShop.BLL
{
    public class StripeData
    {
        public bool IsTest { get; set; }

        public string Email { get; set; }

        public Guid ProductId { get; set; }

        public IProduct Product { get; set; }

        public string Currency { get; set; }

        public long Quantity { get; set; }

        public string CancelUrl { get; set; }

        public string SuccessUrl { get; set; }

        public string StripeJson { get; set; }

        public Session StripeCheckoutSession { get; set; }

        public HttpRequest HttpRequest { get; set; }

        public HttpResponse HttpResponse { get; set; }

        public string StripeEventType { get; set; }

        public IActionResult ActionResult { get; set; }
    }
}