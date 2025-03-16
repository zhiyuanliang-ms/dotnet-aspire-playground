using Azure;
using Azure.Identity;
using Azure.Core;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp
{
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

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var fakeEndpoint = new Uri("http://localhost:7099");
            var credential = new EmptyTokenCredential();
            //var credential = new DefaultAzureCredential(); // fail

            var fakeConnectionString = "Endpoint=https://localhost:7099;Id=xxxx;Secret=xxxx";

            //var client = new ConfigurationClient(fakeEndpoint, credential);

            var client = new ConfigurationClient(fakeConnectionString);

            var selector = new SettingSelector()
            {
                KeyFilter = "*",
                LabelFilter = "\0"
            };

            AsyncPageable<ConfigurationSetting> pageableSettings = client.GetConfigurationSettingsAsync(selector);

            Console.WriteLine("Configuration settings from SDK:");
            await foreach (Page<ConfigurationSetting> page in pageableSettings.AsPages().ConfigureAwait(false))
            {
                using Response response = page.GetRawResponse();

                foreach (ConfigurationSetting setting in page.Values)
                {
                    Console.WriteLine($"{setting.Key}: {setting.Value}");
                }

            }

            //var builder = new ConfigurationBuilder();
            //builder.AddAzureAppConfiguration(options =>
            //{
            //    options.Connect(fakeConnectionString);
            //});
            //var config = builder.Build();
            //var key = "message";
            //Console.WriteLine("Key values from provider:");
            //Console.WriteLine($"{key}: {config[key]}");
        }
    }
}
