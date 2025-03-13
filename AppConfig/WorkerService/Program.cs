using Aspire.Azure.Configuration.AppConfiguration;
using Microsoft.FeatureManagement;

namespace WorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddAzureAppConfiguration("aspire-appconfig", options =>
        {
            options.UseFeatureFlags();
            builder.Services.AddSingleton(options.GetRefresher());

        });
        builder.Services.AddFeatureManagement();
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}