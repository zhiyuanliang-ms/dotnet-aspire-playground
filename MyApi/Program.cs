using Azure.Data.AppConfiguration;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

var settingsStore = new ConcurrentDictionary<string, ConfigurationSetting>();
settingsStore["key1"] = new ConfigurationSetting("key1", "some value1");
settingsStore["key2"] = new ConfigurationSetting("key2", "some value2");
settingsStore["message"] = new ConfigurationSetting("message", "hello world");
settingsStore[".appconfig.featureflag/Beta"] = new ConfigurationSetting(".appconfig.featureflag/Beta", @"{""id"": ""Beta"", ""enabled"": true}");

builder.Services.AddSingleton(settingsStore);

var app = builder.Build();

app.UseHttpsRedirection();

//app.MapGet("/", () =>
//{
//    return "Hello World!";
//});

app.MapGet("/", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>App Configuration Store</title>
    <script>
        async function loadSettings() {
            const response = await fetch('/kv');
            const data = await response.json();
            const list = document.getElementById('settingsList');
            list.innerHTML = '';
            data.items.forEach(setting => {
                const li = document.createElement('li');
                li.innerHTML = `<strong>${setting.key}</strong>: ${setting.value} 
                    <button onclick='editSetting(${JSON.stringify(setting)})'>Edit</button>
                    <button onclick='deleteSetting(\""${setting.key}\"")'>Delete</button>`;
                list.appendChild(li);
            });
        }

        function editSetting(setting) {
            document.getElementById('key').value = setting.key;
            document.getElementById('value').value = setting.value;
        }

        async function updateSetting() {
            const key = document.getElementById('key').value;
            const value = document.getElementById('value').value;
            const setting = {
                key: key,
                label: null,
                value: value,
                content_type: null,
                tags: {}
            };

            const response = await fetch('/kv/' + key, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(setting)
            });
            
            if(response.ok) {
                alert('Updated successfully!');
                loadSettings();
            } else {
                alert('Update failed.');
            }
        }

        async function addSetting() {
            const key = document.getElementById('key').value;
            const value = document.getElementById('value').value;
            const setting = {
                key: key,
                label: null,
                value: value,
                content_type: null,
                tags: {}
            };

            const response = await fetch('/kv', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(setting)
            });

            if(response.ok) {
                alert('Added successfully!');
                loadSettings();
            } else {
                alert('Add failed.');
            }
        }

        async function deleteSetting(key) {
            const response = await fetch('/kv/' + key, {
                method: 'DELETE'
            });

            if(response.ok) {
                alert('Deleted successfully!');
                loadSettings();
            } else {
                alert('Delete failed.');
            }
        }

        window.onload = loadSettings;
    </script>
</head>
<body>
    <h1>App Configuration Store</h1>
    <ul id='settingsList'></ul>
    <hr />
    <h2>Add / Update Setting</h2>
    <div>
        <label for='key'>Key:</label>
        <input type='text' id='key' placeholder='Enter key'/>
    </div>
    <div>
        <label for='value'>Value:</label>
        <input type='text' id='value' placeholder='Enter value'/>
    </div>
    <div>
        <button onclick='addSetting()'>Add</button>
        <button onclick='updateSetting()'>Update</button>
    </div>
</body>
</html>
    ");
});

app.MapGet("/kv", (ConcurrentDictionary<string, ConfigurationSetting> store, HttpContext httpContext) =>
{
    string etag = $"\"{DateTime.Now.ToString()}\"";
    httpContext.Response.Headers.Append("etag", etag);

    var response = new
    {
        items = store.Values
    };

    return Results.Ok(response);
});

app.MapGet("/kv/{*key}", (string key, ConcurrentDictionary<string, ConfigurationSetting> store) =>
{
    if (store.TryGetValue(key, out var setting))
    {
        return Results.Ok(setting);
    }
    return Results.NotFound($"Setting with key '{key}' not found.");
});

app.MapPost("/kv", (ConfigurationSetting newSetting, ConcurrentDictionary<string, ConfigurationSetting> store) =>
{
    if (store.ContainsKey(newSetting.Key))
    {
        return Results.Conflict($"Setting with key '{newSetting.Key}' already exists.");
    }
    store[newSetting.Key] = newSetting;
    return Results.Created($"/kv/{newSetting.Key}", newSetting);
});

app.MapPut("/kv/{*key}", (string key, ConfigurationSetting updatedSetting, ConcurrentDictionary<string, ConfigurationSetting> store) =>
{
    if (key != updatedSetting.Key)
    {
        return Results.BadRequest("Key in URL doesn't match request body.");
    }

    store[key] = updatedSetting;
    return Results.Ok(updatedSetting);
});

app.MapDelete("/kv/{*key}", (string key, ConcurrentDictionary<string, ConfigurationSetting> store) =>
{
    if (store.TryRemove(key, out _))
    {
        return Results.NoContent();
    }
    return Results.NotFound($"Setting with key '{key}' not found.");
});

app.Run();
