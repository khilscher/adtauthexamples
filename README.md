# Azure Digital Twins v2 authentication examples

This repo contains a Visual Studio solution with several C# examples for authenticating with [**Microsoft Azure Digital Twins**](https://docs.microsoft.com/en-us/azure/digital-twins/overview) service using the [**Azure.Identity**](https://docs.microsoft.com/en-us/dotnet/api/azure.identity?view=azure-dotnet) and the [**Azure Digital Twins**](https://www.nuget.org/packages/Azure.DigitalTwins.Core/1.0.0-preview.3) SDKs.

For interactive applications, use the Interactive example. For non-interactive applications, use either Client Secret or Managed Identities. Ensure AAD is setup appropriately for the authentication type you choose. See **AAD Configuration** below.

You can also use ```DefaultAzureCredential``` which provides a default TokenCredential authentication flow for applications that will be deployed to Azure. The following credential types if enabled will be tried, in order:

- EnvironmentCredential
- ManagedIdentityCredential
- SharedTokenCacheCredential
- InteractiveBrowserCredential

However, I've found that ```DefaultAzureCredential``` can also slow down your auth code as it tries the various credential types. So if you know which credential type you are going to use, I'd recommend just using that specific credential type class as shown in the following examples.

## Interactive

```
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
```

## Client Secret

```
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
```

## Managed Identity

```
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
```
## AAD Configuration




