namespace BingoOverlay.Models.Enums
{
    public enum TwitchUserPermission
    {
        None,
        Moderator,
        Broadcaster,
        Vip,
    }

    public static class TwitchUserPermissionExtensions
    {
        public static string ToFriendlyString(this TwitchUserPermission role)
        {
            return role switch
            {
                TwitchUserPermission.Broadcaster => "broadcaster",
                TwitchUserPermission.Moderator => "moderator",
                TwitchUserPermission.Vip => "vip",
                _ => "unknown"
            };
        }
    }
}
