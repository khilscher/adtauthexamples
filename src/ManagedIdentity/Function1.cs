using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.DigitalTwins.Core;
using System.Net.Http;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core.Models;
using Azure;

namespace ManagedIdentity
{
    public static class Function1
    {
        const string adtAppId = "https://digitaltwins.azure.net";
        private static string adtInstanceUrl = "https://<your ADT instance>";
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            DigitalTwinsClient client;
            AsyncPageable<ModelData> modelList;

            try
            {
                ManagedIdentityCredential cred = new ManagedIdentityCredential(adtAppId);

                DigitalTwinsClientOptions opts = new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) };

                client = new DigitalTwinsClient(new Uri(adtInstanceUrl), cred, opts);

                modelList = client.GetModelsAsync(null, true);

                await foreach (ModelData md in modelList)
                {
                    log.LogInformation($"Id: {md.Id}");
                }

                log.LogInformation("Done");
            }
            catch (Exception e)
            {
                log.LogCritical($"Authentication or client creation error: {e.Message}");
                return new BadRequestObjectResult(e.Message);
            }

            return new OkObjectResult(modelList);
        }
    }
}
