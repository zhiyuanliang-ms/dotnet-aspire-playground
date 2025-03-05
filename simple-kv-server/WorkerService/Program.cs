using Azure.Core;

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
        var fakeConnectionString = "Endpoint=https://localhost:7242;Id=xxxx;Secret=xxxx";
        //var fakeConnectionString = "Endpoint=https://kvserver;Id=xxxx;Secret=xxxx"; // fail

        //var fakeEndpoint = new Uri("https://kvserver"); fail
        //var fakeEndpoint = new Uri("https://localhost:7242");
        //var credential = new EmptyTokenCredential();

        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(fakeConnectionString);
        });
        builder.AddServiceDefaults();
        builder.Services.AddHostedService<Worker>();

        builder.Services.AddHttpClient<TestHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://kvserver");
        });

        var host = builder.Build();
        host.Run();
    }
}