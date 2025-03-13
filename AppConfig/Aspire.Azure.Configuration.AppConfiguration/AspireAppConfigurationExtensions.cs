using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Aspire.Azure.Configuration.AppConfiguration
{
    public static class AspireAppConfigurationExtensions
    {
        public static IConfigurationBuilder AddAzureAppConfiguration(
            this IConfigurationManager builder, 
            string connectionName,
            Action<AzureAppConfigurationOptions> action = null)
        {
            return builder.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(builder.GetConnectionString(connectionName)), new AzureCliCredential());
                action?.Invoke(options);
            });
        }
    }
}
