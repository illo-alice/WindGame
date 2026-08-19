using Fusion;
using UnityEngine;

public interface IPlayerSpawnerProvider
{
    void Spawn(
        PlayerRef playerRef,
        NetworkRunner runner,
        NetworkObject prefab,
        Transform spawnPoint
    );

    void Despawn(
        PlayerRef playerRef,
        NetworkRunner runner
    );
}