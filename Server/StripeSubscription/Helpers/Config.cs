using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeSubscription.Helpers
{
    public class Config
    {
        public string StripeApiSecretKey;
        public string SubscriptionSuccessUrl;
        public string SubscriptionFailureUrl;

        public static Config CreateFrom(ExecutionContext context)
        {
            return new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables()
                .Build()
                .AsEnumerable()
                .ToInstance<Config>();
        }

        public bool IsValid(out string errorMessages)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(StripeApiSecretKey))
                errors.AppendFormat("Stripe API secret key not defined in Azure Function setting '{0}'\n", nameof(StripeApiSecretKey));
            //enable this only if using redirection
            //if (string.IsNullOrWhiteSpace(SubscriptionSuccessUrl))
            //    errors.AppendFormat("Success url not defined in Azure Function setting '{0}'\n", nameof(SubscriptionSuccessUrl));
            //else if (Uri.TryCreate(SubscriptionSuccessUrl, UriKind.Absolute, out var _))
            //    errors.AppendFormat("Success url not valid url format in Azure Function setting '{0}'\n", nameof(SubscriptionSuccessUrl));

            //if (string.IsNullOrWhiteSpace(SubscriptionFailureUrl))
            //    errors.AppendFormat("Failure url not defined in Azure Function setting '{0}'\n", nameof(SubscriptionFailureUrl));
            //else if (Uri.TryCreate(SubscriptionFailureUrl, UriKind.Absolute, out var _))
            //    errors.AppendFormat("Failure url not defined in Azure Function setting '{0}'\n", nameof(SubscriptionFailureUrl));

            errorMessages = errors.ToString();

            return errors.Length == 0;
        }

        public static string GetStripeApiSecretKey()
        {
            return "sk_test_51MYsOVSA2hIwWxkOOeAnXODEntknDL7dvEOCwCUv292HUYUao5vhBShC9MQVeLhicFbQsK3NxDyYXvF2laBKnMLQ00HVWWTXGK";
        }
    }
}

