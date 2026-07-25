namespace BingoOverlay.Models
{
    public class Settings
    {
        public int Id { get; set; }

        public bool AllowModerators { get; set; } = true;

        public bool HideOverlayAfterTime { get; set; }

        public int HideOverlaySeconds { get; set; } = 60;


        public bool IsOverlayVisible { get; set; } = true;

        public DateTime LastOverlayActivity { get; set; } = DateTime.UtcNow;
    }
}
