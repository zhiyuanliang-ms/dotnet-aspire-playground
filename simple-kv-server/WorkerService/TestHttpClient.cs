namespace WorkerService
{
    public class TestHttpClient(HttpClient client)
    {
        public async Task<string> GetKvAsync()
        {
            return await client.GetStringAsync("kv");
        }
    }
}
