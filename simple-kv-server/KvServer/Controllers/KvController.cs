using Microsoft.AspNetCore.Mvc;
using Azure.Data.AppConfiguration;

namespace KvServer.Controllers;

[ApiController]
[Route("[controller]")]
public class kvController : ControllerBase
{
    private readonly ILogger<kvController> _logger;
    private ConfigurationSetting[] _kvs =
    {
        new ConfigurationSetting("key1", "some value1"),
        new ConfigurationSetting("key2", "some value2"),
        new ConfigurationSetting("key3", "some value3"),
        new ConfigurationSetting("key4", "some value4"),
        new ConfigurationSetting("key5", "some value5"),
        new ConfigurationSetting("message", "hello world"),
    };

    public kvController(ILogger<kvController> logger)
    {
        _logger = logger;
    }

    public record KeyValueResponse(IEnumerable<ConfigurationSetting> Items);

    [HttpGet]
    public ActionResult<KeyValueResponse> Get()
    {
        return Ok(new KeyValueResponse(_kvs));
    }
}
