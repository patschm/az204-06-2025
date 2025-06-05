
using System;
using System.Threading.Tasks;

#region KeyVault
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault;
using Azure.Identity;
#endregion


namespace KeyVault;

class Program
{
    static string tenentId = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
    static string clientId = "e96ce23c-91ff-407d-92e2-4aefb321d62e";
    static string clientSecret = "";
    static string kvUri = "https://ps-sleutels.vault.azure.net/";

    static async Task Main(string[] args)
    {
        //clientSecret = Environment.GetEnvironmentVariable("GEHEIM");
        await ReadKeyVault();
        
        Console.WriteLine("Done");
        Console.ReadLine();
    }
    private static async Task ReadKeyVault()
    {
        //ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
        //DefaultAzureCredential cred = new DefaultAzureCredential();
        // var cred = new ManagedIdentityCredential("49cd35d4-b9e5-49ad-b477-0711710afb1a");
        // or define your own chain
        var cred = new ChainedTokenCredential(
            new EnvironmentCredential(),
            new VisualStudioCodeCredential(),
            new ClientSecretCredential(tenentId, clientId, clientSecret),
            new ManagedIdentityCredential("49cd35d4-b9e5-49ad-b477-0711710afb1a")
            );


        //var cred = new VisualStudioCredential();
        SecretClient kvClient = new SecretClient(new Uri(kvUri), cred);

        var result = await kvClient.GetSecretAsync("geheim");
        Console.WriteLine($"Hello: {result.Value?.Value}");
    }
}
