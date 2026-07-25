namespace BingoOverlay.Models.Enums
{
    public enum TwitchUserPermission
    {
        None,
        Moderator,
        Broadcaster,
    }

    public static class TwitchUserPermissionExtensions
    {
        public static string ToFriendlyString(this TwitchUserPermission role)
        {
            return role switch
            {
                TwitchUserPermission.Broadcaster => "broadcaster",
                TwitchUserPermission.Moderator => "moderator",
                _ => "unknown"
            };
        }
    }
}
