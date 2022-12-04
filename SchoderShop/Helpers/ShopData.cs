using Microsoft.AspNetCore.Http;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SchoderChain;

namespace SchoderShop.Helpers
{
    public class ShopData
    {
        //public ClaimsPrincipal User { get; set; }

        //public string UserId { get; set; }

        //public string FullName { get; set; }

        public string Email { get; set; }

        //public Models.Account Account { get; set; }

        //public string DonationType { get; set; }

        //public bool IsDonation { get; set; }

        //public Guid ProductId { get; set; }

        //public Models.Product Product { get; set; }

        //public bool IsLive { get; set; }

        public string CancelUrl { get; set; }

        public string SuccessUrl { get; set; }

        //public string StripeJson { get; set; }

        //public Session StripeCheckoutSession { get; set; }

        //public HttpRequest HttpRequest { get; set; }

        //public HttpResponse HttpResponse { get; set; }

        //public string PaymentIntentId { get; set; }

        //public string StripeEventType { get; set; }
    }
}