using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace LastStand.Helpers;
public class LastStandClient
{
    private HttpClient _httpClient;
    public LastStandClient()
    {
        _httpClient = new HttpClient() { BaseAddress = new Uri(Setup.ServerEndpoint) };
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {Setup.Token}");
    }

    public async Task<string?> CommandAsync(string command)
    {
        if (command is null) throw new ArgumentNullException();
        var result = await _httpClient.PostAsJsonAsync($"/Play/Command", new { Command = command });
        if (result.IsSuccessStatusCode is false) throw new Exception("Call to server failed. Make sure you have a valid token");
        else if (result.StatusCode is HttpStatusCode.NoContent) return null;
        var response = await result.Content.ReadAsStringAsync();
        return response;
    }

    public async Task<string?> SendOutAsync(SendRequest send)
    {
        if (send is null) throw new ArgumentNullException();
        var result = await _httpClient.PostAsJsonAsync($"/Play/Send", send);
        if (result.IsSuccessStatusCode is false) throw new Exception("Call to server failed. Make sure you have a valid token");
        else if (result.StatusCode is HttpStatusCode.NoContent) return null;
        var response = await result.Content.ReadAsStringAsync();
        return response;
    }
}
public class SendRequest
{
    public string PlayerName { get; set; }
    public CombatBehaviour CombatBehaviour { get; set; }
    public List<string> Commands { get; set; }
    public List<string> Items { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CombatBehaviour
{
    ignore = 0,
    flee = 1,
    fight = 2
}
