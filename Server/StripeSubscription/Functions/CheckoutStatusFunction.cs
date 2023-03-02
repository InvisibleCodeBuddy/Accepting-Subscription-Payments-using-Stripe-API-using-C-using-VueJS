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
using Stripe;
using Stripe.Checkout;
using StripeSubscription.Helpers;
using StripeSubscription.Model;
using StripeSubscription.Models;

namespace StripeSubscription.Functions
{
    public static class CheckoutStatusFunction
    {
        [FunctionName("CheckoutStatusFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "checkout-session")][RequestBodyType(typeof(GetCheckoutSessionRequest), "request")] HttpRequest req, ILogger log)
        {
            try
            {
                log.LogInformation("get payment intent function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                GetCheckoutSessionRequest request = JsonConvert.DeserializeObject<GetCheckoutSessionRequest>(requestBody);

                StripeConfiguration.ApiKey = Config.GetStripeApiSecretKey();

                var service = new SessionService();

                var session = await service.GetAsync(request.checkout_session_id);
                
                return new OkObjectResult(session);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new OkObjectResult(ex);
            }

        }
    }
}
