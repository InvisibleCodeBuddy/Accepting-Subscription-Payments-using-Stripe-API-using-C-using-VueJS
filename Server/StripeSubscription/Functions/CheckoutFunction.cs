using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using StripeSubscription.Model;
using Stripe.Checkout;
using Stripe;
using System.Collections.Generic;
using StripeSubscription.Models;
using StripeSubscription.Helpers;
using Stripe.Issuing;
using System.Security.Cryptography.Xml;

namespace StripeSubscription.Functions
{    
        public static class CheckoutFunction
        {
        // trial redirect to stripe
            [FunctionName("Checkout")]
            public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "checkout")][RequestBodyType(typeof(SubscriptionForm), "request")] HttpRequest req,
                ILogger log)
            {
                log.LogInformation("checkout function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                SubscriptionForm form = JsonConvert.DeserializeObject<SubscriptionForm>(requestBody);

                StripeConfiguration.ApiKey = StripeConfiguration.ApiKey = Config.GetStripeApiSecretKey();

                var options = new SessionCreateOptions
                {
                    // See https://stripe.com/docs/api/checkout/sessions/create
                    // for additional parameters to pass.
                    // {CHECKOUT_SESSION_ID} is a string literal; do not change it!
                    // the actual Session ID is returned in the query parameter when customer
                    // is redirected to the success page.
                    SuccessUrl = "http://localhost:8080/account?checkout_session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = "http://localhost:8080/account?payment_cancelled=true",
                    PaymentMethodTypes= new List<string> {"card"},
                    PaymentMethodCollection = "always",                   
                    Mode = "subscription",
                    LineItems = new List<SessionLineItemOptions>
                        {
                        new SessionLineItemOptions
                            {
                                Price = form.PriceId,
                                Quantity = 1,
                            },
                            },
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return new OkObjectResult(new ChekoutSessionResponse() { Url = session.Url });
            }
        }    
}
