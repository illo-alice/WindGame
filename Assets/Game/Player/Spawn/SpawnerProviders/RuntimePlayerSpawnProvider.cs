using Fusion;
using UnityEngine;

public sealed class RuntimePlayerSpawnProvider : IPlayerSpawnerProvider
{
    private readonly GameSessionSettings _settings;
    private readonly OnlinePlayerSpawnProvider _online;
    private readonly LocalPlayerSpawnProvider _local;

    public RuntimePlayerSpawnProvider(
        GameSessionSettings settings,
        OnlinePlayerSpawnProvider online,
        LocalPlayerSpawnProvider local)
    {
        _settings = settings;
        _online = online;
        _local = local;
    }

    public void Spawn(
        PlayerRef playerRef,
        NetworkRunner runner,
        NetworkObject prefab,
        Transform spawnPoint)
    {
        Current.Spawn(
            playerRef,
            runner,
            prefab,
            spawnPoint
        );
    }

    public void Despawn(
        PlayerRef playerRef,
        NetworkRunner runner)
    {
        Current.Despawn(
            playerRef,
            runner
        );
    }

    private IPlayerSpawnerProvider Current =>
        _settings.SpawnMode switch
        {
            PlayerSpawnMode.Online => _online,
            PlayerSpawnMode.Local => _local,

            _ => throw new System.ArgumentOutOfRangeException()
        };
}