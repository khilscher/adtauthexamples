using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.DigitalTwins.Core;
using Azure.DigitalTwins.Core.Models;
using Azure;

namespace ClientSecret
{
    class Program
    {
        // Your client / app registration ID
        private static string clientId = "<your client ID>";
        // Your tenant / directory ID
        private static string tenantId = "<your AAD directory ID>";
        // The URL of your instance, starting with the protocol (https://)
        private static string adtInstanceUrl = "https://<your ADT instance>";
        // Your client secret from your app registration
        private static string secret = "<your client secret>";

        static async Task Main(string[] args)
        {
            DigitalTwinsClient client;

            try
            {
                var credential = new ClientSecretCredential(tenantId, clientId, secret);

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
