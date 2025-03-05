using System.Text.Json;
using System.Web;

namespace WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_httpClient != null)
        {
            Console.WriteLine("YES");

            var res = await _httpClient.GetStringAsync("kv");

            Console.WriteLine(res);
        }

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
