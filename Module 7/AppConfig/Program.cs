using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace AppConfig;

internal class Program
{

    const string conStr = "Endpoint=";
    static async Task Main(string[] args)
    {
       // await ReadConfigurationLocalAsync();
        await ReadAppConfigurationAsync();

        Console.ReadLine();

    }

    private static Task ReadConfigurationLocalAsync()
    {
        // From user-secrets
        // dotnet init
        // dotnet user-secrets set APPC "Endpoint=https://ps-config.azconfig.io;Id=LDpu;Secret=zE8JVpCLRSBwkKyE+sHBE1uPnexZTenYGpvY2eQoY04="
        // var constr = configuration["APPC"];
        // Console.WriteLine(  constr);
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json")
               .AddUserSecrets<AppConfig.Program>(true)
               .AddEnvironmentVariables();
        IConfiguration configuration = builder.Build();

        Console.WriteLine(configuration["MySetings:hello"]);
        Console.WriteLine(configuration["KlantA:KeyVault:BackgroundColor"]);
        Console.WriteLine(configuration["ConnectionString"]);
        return Task.CompletedTask;
    }

    private static async Task ReadAppConfigurationAsync()
    {
        var builder = new ConfigurationBuilder();
        //ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
        //var env = Environment.GetEnvironmentVariable("Bla");
        //builder.AddAzureKeyVault(new Uri(kvUri), new DefaultAzureCredential());
        //builder.AddJsonFile("appsettings.json");
        builder.AddAzureAppConfiguration(opts =>
        {
            opts.ConfigureKeyVault(kvopts =>
            {
                kvopts.SetCredential(new DefaultAzureCredential());
            });
            //var omgeving = Environment.GetEnvironmentVariable("OMGEVING");
            //Console.WriteLine(omgeving);
            opts.Connect(conStr).TrimKeyPrefix("AppConfig:").Select(KeyFilter.Any, "Production");
            //    .Select(KeyFilter.Any, "Production")
            //    .TrimKeyPrefix("KeyVault:MySetings:");
            //opts.ConfigureRefresh(refr => refr.Register("*", true));
            opts.UseFeatureFlags(conf =>
            {
                conf.Select(KeyFilter.Any, "Production");
                conf.CacheExpirationInterval = TimeSpan.FromSeconds(10);
            });
        });
        IConfiguration conf = builder.Build();


        Console.WriteLine(conf["MySetings:hello"]);
        //Console.WriteLine(conf["KlantA:KeyVault:BackgroundColor"]);
        //Console.WriteLine(conf["ConnectionString"]);
        Console.WriteLine(conf["Gehiem"]);
        //Task.Delay(35000).ContinueWith(pt=>   Console.WriteLine(conf["MySetings:hello"]));
        //Console.ReadLine();
        //Console.WriteLine($"{conf["Color"]}");
        //Console.WriteLine($"{conf["MySecret"]}");
        //Console.WriteLine($"Hello {conf["ConnectionString"]}");
        //  Console.WriteLine(conf["KlantA:KeyVault:BackgroundColor"]);
        // Console.WriteLine($"{conf["KV"]}");

        IServiceCollection services = new ServiceCollection();
        services.AddFeatureManagement(conf);
        //services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

        using (var svcProvider = services.BuildServiceProvider())
        {
            do
            {
                using (var scope = svcProvider.CreateScope())
                {
                    var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                    if (await featureManager.IsEnabledAsync("FeatureA"))
                    {
                        Console.WriteLine("We have a new feature");
                    }
                    Console.Write(".");
                    await Task.Delay(2000);
                }
            }
            while (true);
        }
    }

    private static async Task ReadAppFeaturesAsync()
    {
        var builder = new ConfigurationBuilder();
        builder.AddAzureAppConfiguration(opts =>
        {
            opts.Connect(conStr)
               .UseFeatureFlags(opts =>
               {
                   opts.CacheExpirationInterval = TimeSpan.FromSeconds(1);
                   opts.Label = "Production";
               });
        });
        IConfiguration conf = builder.Build();

        IServiceCollection services = new ServiceCollection();
        services.AddFeatureManagement(conf);
        services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

        using (var svcProvider = services.BuildServiceProvider())
        {
            do
            {
                using (var scope = svcProvider.CreateScope())
                {
                    var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                    if (await featureManager.IsEnabledAsync("FeatureA"))
                    {
                        Console.WriteLine("We have a new feature");
                    }
                    Console.Write(".");
                    await Task.Delay(2000);
                }
            }
            while (true);
        }
    }
}
