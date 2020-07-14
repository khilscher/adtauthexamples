using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.DigitalTwins.Core;
using Azure.DigitalTwins.Core.Models;
using Azure;

namespace Interactive
{
    class Program
    {
        // Your client / app registration ID
        private static string clientId = "<your client ID>";
        // Your tenant / directory ID
        private static string tenantId = "<your AAD tenant ID>";
        // The URL of your instance, starting with the protocol (https://)
        private static string adtInstanceUrl = "https://<your ADT instance>";

        static async Task Main(string[] args)
        {
            DigitalTwinsClient client;

            try
            {
                var credential = new InteractiveBrowserCredential(tenantId, clientId);

                // Open a browser window and allow user to select which account to authenticate with
                // If you omit this, the browser will only be launched to authenticate the user once, 
                // then will silently acquire access tokens through the users refresh token as long as it's valid.
                // So if you are switching between AAD accounts, keep this uncommented.
                var auth_result = credential.Authenticate();
                Console.WriteLine($"Sucessfully authenticated as: {auth_result.Username}");

                client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential);

                AsyncPageable<ModelData> modelList = client.GetModelsAsync(null, true);

                await foreach (ModelData md in modelList)
                {
                    Console.WriteLine($"Id: {md.Id}");
                }

                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Authentication or client creation error: {e.Message}");
                Environment.Exit(0);
            }
        }
    }
}
