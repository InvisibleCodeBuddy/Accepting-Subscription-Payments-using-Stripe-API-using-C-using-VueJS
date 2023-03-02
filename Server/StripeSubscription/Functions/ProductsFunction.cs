using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JWT.Builder;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using StripeSubscription.Model;
using StripeSubscription.Models;
using StripeSubscription.Helpers;

namespace StripeSubscription.Functions
{
    public static class ProductsFunction
    {        
        [FunctionName("Products")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.User, "get", Route = "products")] HttpRequest req,
               ILogger log)
        {
                        /*
             Move apikey to environment variable and access it like..

            //StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("stripeSK"); 
             
             */
            StripeConfiguration.ApiKey = Config.GetStripeApiSecretKey();
            // Check if we have authentication info.
             ValidateJWT auth = new ValidateJWT(req);

            if (!auth.IsValid)
            {
                return new UnauthorizedResult(); // No authentication info.
            }

            // await UpdateProductLookUpKey();// uncomment this when ever need to update lookup keys - or create a separate method for that

            var options = new PriceListOptions
            {
                //LookupKeys = productList // if have a product lookup key created in stripe and available in application, pass it for retrieving fast
                 Limit=5 // use limit according to the number of products created in the dashboard or pass the list of product lookupKeys
            };           

            var service = new PriceService();

            var prices = service.List(options);

            var plans = (from p in prices.Data
                         select

                       new SubscriptionPlan
                       {
                           Id = p.Id,
                           PlanName = p.LookupKey,
                           Currency = p.Currency,
                           Price = Convert.ToDecimal(p.UnitAmount)/100,
                           BillingInterval = p.Recurring.Interval
                       }).ToList().OrderBy(p=>p.Price);

            return new OkObjectResult(plans);

        }

        private static async Task UpdateProductLookUpKey()
        {
            // we can use lookupkey in price/product to refere product in application.
            // Whiel creating product there is no option in stipe dashboard to directly update the lookup key.. so we can update it using this method
            try
            {

                var options = new PriceListOptions
                {                    
                    Limit=20
                };                

                var service = new PriceService();

                var prices = await service.ListAsync(options);

                //for lookup key updation, priceid getting from stripe and matching with plan using lookup key accordingly 

                foreach (Stripe.Price price in prices)
                {
                    if (price.LookupKey == null)
                    {
                        var priceUpdateOptons = new PriceUpdateOptions
                        {
                            // replace the priceid here with the exact priceid from stripe product dashboad 
                            LookupKey =
                            price.Id == "price_1MZ7kSSA2hIwWxkOuVok0TTZ" ? "Basic" :
                            price.Id == "price_1MZ7oaSA2hIwWxkOVkjiyDY2" ? "Professional1" :
                            price.Id == "price_1MZ7q8SA2hIwWxkOUNo3eLzB" ? "Premium1" : ""                 

                        };

                      await  service.UpdateAsync(price.Id, priceUpdateOptons);
                    }
                }               
            }
            catch (StripeException ex)
            {
                throw ex;
            }
        }

    }


}
