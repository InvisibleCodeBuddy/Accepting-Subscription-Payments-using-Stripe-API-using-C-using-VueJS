using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using System.Web.Http;
using System.Collections.Generic;
using StripeSubscription.Model;
using StripeSubscription.Helpers;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using StripeSubscription.Data;
using StripeSubscription.Models;

namespace StripeSubscription.Functions
{
    public static class SubscriptionFunction
    {
        [FunctionName("Subscription")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.User, "post", Route = "subscribe")][RequestBodyType(typeof(SubscriptionForm), "request")] HttpRequest req, ILogger log, ExecutionContext context)
        {
            try
            {
                log.LogInformation("subscription function processed a request.");
            //var config = Config.CreateFrom(context);

            //if (!config.IsValid(out var configErrorMessage))

            //   return new BadRequestErrorMessageResult(configErrorMessage);

            //var form = SubscriptionForm.CreateFrom(request);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            SubscriptionForm form = JsonConvert.DeserializeObject<SubscriptionForm>(requestBody);

            if (!form.IsValid(out string errors))
               
                return new BadRequestErrorMessageResult(errors);

            
                StripeConfiguration.ApiKey = StripeConfiguration.ApiKey = Config.GetStripeApiSecretKey();

                var subscriptionCreateResponse =  await CreateCustomerSubscriptionAtStripe(form);

                var responseId = subscriptionCreateResponse.SubscriptionId;

                return new OkObjectResult(subscriptionCreateResponse);
            }
            catch(Exception ex)
            {
                log.LogInformation("Failed to create subscription.");
                log.LogInformation(ex.Message);
                return new OkObjectResult("Failed to create subscription");
             
            }
        }
        public static async Task<SubscriptionCreateResponse> CreateCustomerSubscriptionAtStripe(SubscriptionForm subscriptionForm)
        {
            // first create customer at stripe

            List < User > userList = new List<User>();

            userList = UserData.GetUserList();

            var requestedUser = userList?.Find(u => u.Id == subscriptionForm.CustomerId);

            if (requestedUser != null)
            {
                Stripe.Customer customer= await CreateStripeCustomer(requestedUser); // create user object which 

                var customerId = customer.Id; // store this id in db for getting customer subscriptions from stripe

                // pass the customer Id with the selected plan to create subscription

                var paymentSettings = new SubscriptionPaymentSettingsOptions
                {
                    SaveDefaultPaymentMethod = "on_subscription",
                };

                // Create the subscription. Note we're expanding the Subscription's
                // latest invoice and that invoice's payment_intent
                // so we can pass it to the front end to confirm the payment

                List<SubscriptionItemOptions> subscriptionItemOptions = new List<SubscriptionItemOptions>();

                List<SubscriptionAddInvoiceItemOptions> subcriptionInvoiceItemOptions = new List<SubscriptionAddInvoiceItemOptions>();

                SubscriptionCreateOptions subscriptionOptions = new SubscriptionCreateOptions();

                   // this is the price_ids included in the product. single product may include multiple price components, which will go for recurring
                  
                // here we assumed there is only one price component
                subscriptionItemOptions.Add(new SubscriptionItemOptions
                    {
                        Price = subscriptionForm.PriceId,
                        Quantity = 1, // use dynamic if it changes
                        //TaxRates = request.TaxRates // if there is tax for the product
                    });

                // this is price components  which'll charge at the time of subscription, but is not recurring

                // in the example we don't have any one time charges, 

                //foreach (SubscriptionPrice subscriptionPrice in request.InvoicePriceIds)
                //{
                //    subcriptionInvoiceItemOptions.Add(new SubscriptionAddInvoiceItemOptions
                //    {
                //        Price = subscriptionPrice.SubcriptionPriceId,
                //        Quantity = subscriptionPrice.Quantity,
                //        //TaxRates = request.TaxRates
                //    });
                //}

                subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = customer.Id, //required

                    Items = subscriptionItemOptions, //required                  

                    // AddInvoiceItems = subcriptionInvoiceItemOptions,
                    // set TrialEnd , BillingCycleAnchor  properties if there is trial period and billing cycle adjustment

                    PaymentSettings = paymentSettings,
                    PaymentBehavior = "default_incomplete", 
                    CollectionMethod="charge_automatically"
                };

                subscriptionOptions.AddExpand("latest_invoice.payment_intent");
                
                var subscriptionCreateResponse = await CreateSubscription(subscriptionOptions);

                return subscriptionCreateResponse;
            }
            else
            {
                return null;         
            }
        }

        private static async Task<Stripe.Customer> CreateStripeCustomer(User customer)
        {
            try
            {
                var options = new CustomerCreateOptions
                {
                    Email = customer.Email,
                    Name = customer.Name,
                    Description = customer.Id.ToString(),
                    Address = new AddressOptions
                    {
                        Line1 = customer.Street,
                        Line2 = customer.Apartment,
                        City = customer.City,
                        State = customer.State,
                        Country = customer.Country,
                        PostalCode = customer.Zip
                    },

                };

                var stripeCustomer = await new CustomerService()
                .CreateAsync(options);
                return stripeCustomer;
            }
            catch (StripeException ex)
            {   
                throw ex;
            }           
        }

        private static async Task<Models.SubscriptionCreateResponse> CreateSubscription(SubscriptionCreateOptions subscriptionOptions)
        {
            try
            {
                Stripe.Subscription subscription = await new SubscriptionService().CreateAsync(subscriptionOptions);

                SubscriptionCreateResponse response = new SubscriptionCreateResponse();
                if (subscription != null)
                {
                    response = new Models.SubscriptionCreateResponse
                    {
                        SubscriptionId = subscription.Id,                       
                        ClientSecret = subscription.LatestInvoice.PaymentIntent.ClientSecret,
                        IsSubscribed = true,
                        Message = "Subcription created Successfully"
                    };
                     
                }
                else
                {
                    response = new Models.SubscriptionCreateResponse
                    {
                        SubscriptionId = null,
                        IsSubscribed = false,
                        Message = "Failed to create Subcription"
                    };
                }
                return response;

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // use this method only if there is taxes for specific states and use logic to apply tax for customers belonging to corresponding states
        public static StripeList<TaxRate> GetTaxDetails()
        {
            try
            {
                var service = new TaxRateService();

                StripeList<TaxRate> taxRates = service.List();
               
                return taxRates;
            }
            catch (StripeException ex)
            {
               
                throw ex;
            }           

        }

    }
    
}

//Go to Properties -> Debug -> Application arguments ->

//host start --build --port 7071 --cors * --pause-on-error

