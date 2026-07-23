namespace BingoOverlay.Models
{
    public class TwitchAuth
    {
        public int Id { get; set; }

        public string AccessToken { get; set; } = "";

        public string RefreshToken { get; set; } = "";

        public string BroadcasterId { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public DateTime ExpiresAt { get; set; }
    }
}
