using Azure.Data.AppConfiguration;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return "Hello World!";
});

var kvs = new ConfigurationSetting[]
{
    new ConfigurationSetting("key1", "some value1"),
    new ConfigurationSetting("key2", "some value2"),
    new ConfigurationSetting("key3", "some value3"),
    new ConfigurationSetting("key4", "some value4"),
    new ConfigurationSetting("key5", "some value5"),
    new ConfigurationSetting("message", "hello world"),
    new ConfigurationSetting(".appconfig.featureflag/Beta", @"{""id"": ""Beta"", ""enabled"": true}")
};

app.MapGet("/kv", () =>
{
    return new KeyValueResponse(kvs);
});

app.Run();

record KeyValueResponse(IEnumerable<ConfigurationSetting> Items);