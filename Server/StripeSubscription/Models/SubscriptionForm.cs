using Microsoft.AspNetCore.Http;
using StripeSubscription.Helpers;
using StripeSubscription.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeSubscription.Model
{
    public class SubscriptionForm
    {
        public int CustomerId;        
        public string PriceId;

        public static SubscriptionForm CreateFrom(HttpRequest request)
        {
            return request.Form.ToDictionary(k => k.Key, v => v.Value.FirstOrDefault()).ToInstance<SubscriptionForm>();
        }

        public bool IsValid(out string errorMessages)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(CustomerId.ToString()))
                errors.AppendFormat("Subscriber CustomerId not found in '{0}' form value\n", nameof(CustomerId));   

            if (string.IsNullOrWhiteSpace(PriceId))
                errors.AppendFormat("Subscription plan ID not found in '{0}' form value\n", nameof(PriceId));

            errorMessages = errors.ToString();

            return errors.Length == 0;
        }
    }
}