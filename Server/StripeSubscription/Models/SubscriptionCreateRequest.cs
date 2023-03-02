using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StripeSubscription.Models
{
    public class SubscriptionCreateRequest
    {
        public string StripeCustomerID { get; set; }

        public bool HasValidPromoCode { get; set; }

        public bool IsNewOrder { get; set; }

        public string PromoCode { get; set; }

        public List<SubscriptionPrice> SubcriptionPriceIds { get; set; }

        public List<SubscriptionPrice> InvoicePriceIds { get; set; }

        public List<string> TaxRates { get; set; }

    }

    public class SubscriptionPrice
    {
        public string SubcriptionPriceId { get; set; }
        public int Quantity { get; set; }
    }

    public class SubscriptionCreateResponse
    {       
        public string SubscriptionId { get; set; }
       
        public bool IsSubscribed { get; set; }

        public string ClientSecret { get; set; }        

        public string Message { get; set; }
        
    }

    public class ChekoutSessionResponse
    {
        public string Url { get; set; }
   }

    public class CreatePaymentIntentResponse
    {
        public string ClientSecret { get; set; }
    }

    public class GetPaymentIntentRequest
    {
        public string payment_intent_id { get; set; }
    }

    public class GetCheckoutSessionRequest
    {
        public string checkout_session_id { get; set; }
    }
}