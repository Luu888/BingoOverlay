namespace BingoOverlay.Models
{
    public class BingoAppearance
    {
        public int Id { get; set; }

        public string TileColor { get; set; } = "#ff8fd8";

        public string TileColorEnd { get; set; } = "#c99cff";

        public string CompletedColor { get; set; } = "#7dffb2";

        public string CompletedColorEnd { get; set; } = "#32d583";

        public string TextColor { get; set; } = "#ffffff";

        public int BorderRadius { get; set; } = 24;

        public int TileSize { get; set; } = 120;
    }
}
