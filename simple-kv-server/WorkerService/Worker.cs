using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

namespace WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationRefresher _refresher;
    private readonly IVariantFeatureManager _featureManager;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IConfigurationRefresher refresher, IVariantFeatureManager featureManager)
    {
        _logger = logger;
        _configuration = configuration;
        _refresher = refresher;
        _featureManager = featureManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Configuration values:");
        foreach (var pair in _configuration.AsEnumerable())
        {
            Console.WriteLine($"{pair.Key} = {pair.Value}");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await _refresher.TryRefreshAsync(stoppingToken);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Beta is enabled: {enabled}", await _featureManager.IsEnabledAsync("Beta", stoppingToken));
                _logger.LogInformation("Message: {message}", _configuration["message"]);
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(10000, stoppingToken);
        }
    }
}
