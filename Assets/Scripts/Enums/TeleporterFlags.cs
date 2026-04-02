namespace Assets.Scripts.Enums
{
    [Flags]
    public enum TeleporterFlags
    {
        None = 0,
        AllowPickups = 1,
        AllowPlayers = 2,
        AllowProjectiles = 4
    }
}