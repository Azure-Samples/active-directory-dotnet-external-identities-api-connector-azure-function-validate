using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sample.ExternalIdentities
{
    public class SignUpValidation
    {
        [Function("SignUpValidation")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req, FunctionContext executionContext)
        {
            var _logger = executionContext.GetLogger("HttpFunction");

            // Allowed domains
            string[] allowedDomain = { "fabrikam.com", "fabricam.com" };
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);

            // Check HTTP basic authorization
            if (!Authorize(req, _logger))
            {
                _logger.LogWarning("HTTP basic authentication validation failed.");

                response = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);

                response.Headers.Add("Content-Type", "application/json");

                return response;
            }

            // Get the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // If input data is null, show block page
            if (data == null)
            {
                await response.WriteAsJsonAsync(new ResponseContent("ShowBlockPage", "There was a problem with your request."));

                return response;
            }

            // Print out the request body
            _logger.LogInformation("Request body: " + requestBody);

            // Get the current user language 
            string language = (data.ui_locales == null || data.ui_locales.ToString() == "") ? "default" : data.ui_locales.ToString();
            _logger.LogInformation($"Current language: {language}");

            // If email claim not found, show block page. Email is required and sent by default.
            if (data.email == null || data.email.ToString() == "" || data.email.ToString().Contains("@") == false)
            {
                await response.WriteAsJsonAsync(new ResponseContent("ShowBlockPage", "Email name is mandatory."));

                return response;
            }

            // Get domain of email address
            string domain = data.email.ToString().Split("@")[1];

            // Check the domain in the allowed list
            if (!allowedDomain.Contains(domain.ToLower()))
            {
                _logger.LogInformation("BlockPage Response");

                await response.WriteAsJsonAsync(new ResponseContent("ShowBlockPage", $"You must have an account from '{string.Join(", ", allowedDomain)}' to register as an external user for Contoso."));

                return response;
            }

            // If displayName claim doesn't exist, or it is too short, show validation error message. So, user can fix the input data.
            if (data.displayName == null || data.displayName.ToString().Length < 5)
            {
                _logger.LogInformation("BlockPage Response");
                response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);

                await response.WriteAsJsonAsync(new ResponseContent("ValidationError", "Please provide a Display Name with at least five characters."));

                return response;
            }

            // Input validation passed successfully, return `Allow` response.
            // TO DO: Configure the claims you want to return

            await response.WriteAsJsonAsync(new ResponseContent()
            {
                jobTitle = "This value return by the API Connector"//,
                // You can also return custom claims using extension properties.
                //extension_CustomClaim = "my custom claim response"
            });

            return response;
        }

        private static bool Authorize(HttpRequestData req, ILogger log)
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
            if (!req.Headers.Contains("Authorization"))
            {
                log.LogWarning("Missing HTTP basic authentication header.");
                return false;
            }

            // Read the authorization header
            var auth = req.Headers.GetValues("Authorization").First();
            log.LogInformation(auth);
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
