using System.Text.Json.Serialization;

namespace BingoOverlay.Models;

public class TwitchUserResponse
{
    [JsonPropertyName("data")]
    public List<TwitchUser> Data { get; set; } = [];
}


public class TwitchUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = "";
}