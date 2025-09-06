using System.Text;
using System.Text.Json;

namespace T;

public static class Webhook
{
    public static async Task Send(string message)
    {
        if (string.IsNullOrWhiteSpace(Config.webhook)) return;

        try
        {
            using HttpClient client = new();

            var payload = new { content = $"```{message}```" };
            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(Config.webhook, content);

            if (!response.IsSuccessStatusCode)
                Debug.LogWarning($"Error {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
