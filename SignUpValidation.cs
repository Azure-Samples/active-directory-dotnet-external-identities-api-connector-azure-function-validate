using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace Sample.ExternalIdentities
{
    public static class SignUpValidation
    {
        [FunctionName("SignUpValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Allowed domains
            string[] allowedDomain = { "fabrikam.com", "fabricam.com" };

            // Check HTTP basic authorization
            if (!Authorize(req, log))
            {
                log.LogWarning("HTTP basic authentication validation failed.");
                return (ActionResult)new UnauthorizedResult();
            }

            // Get the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // If input data is null, show block page
            if (data == null)
            {
                return (ActionResult)new OkObjectResult(new ResponseContent("ShowBlockPage", "There was a problem with your request."));
            }

            // Print out the request body
            log.LogInformation("Request body: " + requestBody);

            // Get the current user language 
            string language = (data.ui_locales == null || data.ui_locales.ToString() == "") ? "default" : data.ui_locales.ToString();
            log.LogInformation($"Current language: {language}");

            // If email claim not found, show block page. Email is required and sent by default.
            if (data.email == null || data.email.ToString() == "" || data.email.ToString().Contains("@") == false)
            {
                return (ActionResult)new OkObjectResult(new ResponseContent("ShowBlockPage", "Email name is mandatory."));
            }

            // Get domain of email address
            string domain = data.email.ToString().Split("@")[1];

            // Check the domain in the allowed list
            if (!allowedDomain.Contains(domain.ToLower()))
            {
                return (ActionResult)new OkObjectResult(new ResponseContent("ShowBlockPage", $"You must have an account from '{string.Join(", ", allowedDomain)}' to register as an external user for Contoso."));
            }

            // If displayName claim doesn't exist, or it is too short, show validation error message. So, user can fix the input data.
            if (data.displayName == null || data.displayName.ToString().Length < 5)
            {
                return (ActionResult)new BadRequestObjectResult(new ResponseContent("ValidationError", "Please provide a Display Name with at least five characters."));
            }

            // Input validation passed successfully, return `Allow` response.
            // TO DO: Configure the claims you want to return
            return (ActionResult)new OkObjectResult(new ResponseContent() { 
                jobTitle = "This value return by the API Connector"//,
                // You can also return custom claims using extension properties.
                //extension_CustomClaim = "my custom claim response"
            });
        }

        private static bool Authorize(HttpRequest req, ILogger log)
        {
            // Get the environment's credentials 
            string username = System.Environment.GetEnvironmentVariable("BASIC_AUTH_USERNAME", EnvironmentVariableTarget.Process);
            string password = System.Environment.GetEnvironmentVariable("BASIC_AUTH_PASSWORD", EnvironmentVariableTarget.Process);

            // Returns authorized if the username is empty or not exists.
            if (string.IsNullOrEmpty(username))
            {
                log.LogInformation("HTTP basic authentication is not set.");
                return true;
            }

            // Check if the HTTP Authorization header exist
            if (!req.Headers.ContainsKey("Authorization"))
            {
                log.LogWarning("Missing HTTP basic authentication header.");
                return false;
            }

            // Read the authorization header
            var auth = req.Headers["Authorization"].ToString();

            // Ensure the type of the authorization header id `Basic`
            if (!auth.StartsWith("Basic "))
            {
                log.LogWarning("HTTP basic authentication header must start with 'Basic '.");
                return false;
            }

            // Get the the HTTP basinc authorization credentials
            var cred = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');

            // Evaluate the credentials and return the result
            return (cred[0] == username && cred[1] == password);
        }
    }
}
