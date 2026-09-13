namespace BingoOverlay.Models;

public class CountdownSettings
{
    public int Id { get; set; } = 1;

    public int DurationSeconds { get; set; } = 600;

    public int RemainingSeconds { get; set; } = 600;

    public bool IsRunning { get; set; }

    public DateTimeOffset? EndsAt { get; set; }

    public string DigitColor { get; set; } = "#ffffff";

    public string BackgroundColor { get; set; } = "#000000";

    public bool TransparentBackground { get; set; } = true;
}