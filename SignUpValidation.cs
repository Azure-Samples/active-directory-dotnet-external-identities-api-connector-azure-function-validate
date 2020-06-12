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
            string[] allowedDomain = {"fabrikam.com" ,"contoso.com"};

            // Get the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // If input data is null, show block page
            if (data == null)
            {
                return (ActionResult)new OkObjectResult(new ResponseContent("ShowBlockPage", "SingUp-Validation-01", "Invalid input data."));
            }

            // Print out the request body
            log.LogInformation("Request body: " + requestBody);

            // Get the current user language 
            string language = (data.ui_locales == null || data.ui_locales.ToString() == "") ? "default" : data.ui_locales.ToString();
            log.LogInformation($"Current language: {language}");

            // If email claim not found, show validation error message. So, user can fix the input data
            if (data.email == null || data.email.ToString() == "" || data.email.ToString().Contains("@") == false)
            {
                return (ActionResult)new BadRequestObjectResult(new ResponseContent("ValidationError", "SingUp-Validation-03", "Email name is mandatory."));
            }

            // get domain of email address
            string domain = data.email.ToString().Split("@")[1];

            // If email claim not found, show validation error message. So, user can fix the input data
            if ( !allowedDomain.Contains(domain.ToLower()) )
            {
                return (ActionResult)new BadRequestObjectResult(new ResponseContent("ValidationError", "SingUp-Validation-04", $"You must have an account from '{string.Join(", " ,allowedDomain)}' to register as an external user for Contoso."));
            }

            // If jobTitle claim doesn't exist, or it is too short, show validation error message. So, user can fix the input data.
            if (data.jobTitle == null || data.jobTitle.ToString() == "" || data.jobTitle.ToString().Length < 4)
            {
                return (ActionResult)new BadRequestObjectResult(new ResponseContent("ValidationError", "SingUp-Validation-05", "Please provide a job title of length greater than 4."));
            }

            // Input validation passed successfully, return `Allow` response.
            return (ActionResult)new OkObjectResult(new ResponseContent());
        }
    }
}
