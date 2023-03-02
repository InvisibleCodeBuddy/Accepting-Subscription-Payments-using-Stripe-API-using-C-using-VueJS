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
using Stripe;
using StripeSubscription.Helpers;
using StripeSubscription.Models;
using System.Collections.Generic;

namespace StripeSubscription.Functions
{
    public static class PaymentIntentFunction
    {
        [FunctionName("PaymentIntentFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "payment-intent")][RequestBodyType(typeof(GetPaymentIntentRequest), "request")] HttpRequest req,ILogger log)
        {
            try
            {
                log.LogInformation("get payment intent function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                GetPaymentIntentRequest request = JsonConvert.DeserializeObject<GetPaymentIntentRequest>(requestBody);

                StripeConfiguration.ApiKey = Config.GetStripeApiSecretKey();

                var paymentIntentService = new PaymentIntentService();

                var paymentIntent = await paymentIntentService.GetAsync(request.payment_intent_id);

                return new OkObjectResult(paymentIntent);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                return new OkObjectResult(ex);
            }          
           
        }
    }
}
