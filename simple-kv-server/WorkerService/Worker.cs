using System.Text.Json;
using System.Web;

namespace WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly TestHttpClient _client;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, TestHttpClient client)
    {
        _logger = logger;
        _configuration = configuration;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine(await _client.GetKvAsync());

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation(_configuration["message"] ?? "No data.");
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
