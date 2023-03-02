using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StripeSubscription.Model;
using StripeSubscription.Helpers;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using System.Collections.Generic;
using StripeSubscription.Data;
using Stripe;

namespace StripeSubscription.Functions
{
    public static class AuthenticateFunction
    {
        [FunctionName(nameof(Authenication))]
        public static async Task<IActionResult> Authenication(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")][RequestBodyType(typeof(UserCredentials), "request")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            // TODO: Perform custom authentication here; we're just using a simple hard coded check for this example

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserCredentials authData = JsonConvert.DeserializeObject<UserCredentials>(requestBody);
            List<User> userList= new List<User>();

            userList = UserData.GetUserList();

            bool authenticated = userList?.Exists( u=>u.UserName==authData.UserName && u.Password==authData.Password) ?? false;
           
            if (!authenticated)
            {
                return await Task.FromResult(new UnauthorizedResult()).ConfigureAwait(false);
            }
            else
            {
                var user = userList?.Find(u => u.UserName == authData.UserName && u.Password == authData.Password);
                 
                GenerateJWTToken generateJWTToken = new();

                string token = generateJWTToken.IssuingJWT(authData.UserName);

                AuthData auth = new AuthData { User = user, Token = token };

                return await Task.FromResult(new OkObjectResult(auth)).ConfigureAwait(false);
            }

        }
    }
}
