using Azure.Core;
using Microsoft.FeatureManagement;

namespace WorkerService;
internal class EmptyTokenCredential : TokenCredential
{
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(string.Empty, DateTimeOffset.MaxValue);
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new ValueTask<AccessToken>(new AccessToken(string.Empty, DateTimeOffset.MaxValue));
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var fakeConnectionString = "Endpoint=https://localhost:7099;Id=xxxx;Secret=xxxx";
        //var fakeConnectionString = "Endpoint=https://aspireappconfig-gdltoyolrk27a.azconfig.io;Id=33T+;Secret=3o9yOfZqqzcBkp4wa9yEDZRAj2VohEfocJ7XcNbBrsUlML8YrUFfJQQJ99BCACYeBjFJhn2FAAACAZAC1scw";
        //var fakeConnectionString = "Endpoint=https://kvserver;Id=xxxx;Secret=xxxx"; // fail

        //var fakeEndpoint = new Uri("https://kvserver"); fail
        //var fakeEndpoint = new Uri("https://localhost:7242");
        //var credential = new EmptyTokenCredential();

        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(fakeConnectionString);
            options.UseFeatureFlags();
            options.ConfigureRefresh(refresh =>
            {
                refresh.RegisterAll();
            });
            builder.Services.AddSingleton(options.GetRefresher());
        });
        builder.Services.AddFeatureManagement();

        //builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        //builder.Services.AddHttpClient<TestHttpClient>(client =>
        //{
        //    client.BaseAddress = new Uri("https://kvserver");
        //});

        var host = builder.Build();
        host.Run();
    }
}