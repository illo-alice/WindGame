public enum PlayerSpawnMode
{
    Online = 0,
    Local = 1,
}

public sealed class GameSessionSettings
{
    public PlayerSpawnMode SpawnMode { get; private set; } =
        PlayerSpawnMode.Online;

    public void SetSpawnMode(PlayerSpawnMode mode)
    {
        SpawnMode = mode;
    }
}
