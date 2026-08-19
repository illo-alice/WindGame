using Fusion;
using UnityEngine;

public sealed class OnlinePlayerSpawnProvider :
    IPlayerSpawnerProvider
{
    public void Spawn(
        PlayerRef playerRef,
        NetworkRunner runner,
        NetworkObject prefab,
        Transform spawnPoint)
    {
        if (!runner.IsServer)
            return;

        var playerObject = runner.Spawn(
            prefab: prefab,
            position: spawnPoint.position,
            rotation: Quaternion.identity,
            inputAuthority: playerRef,
            onBeforeSpawned: (_, networkObject) =>
            {
                networkObject
                    .GetComponent<LocalPlayerSlot>()
                    .SetIndex(0);
            }
        );

        runner.SetPlayerObject(
            playerRef,
            playerObject
        );
    }

    public void Despawn(
        PlayerRef playerRef,
        NetworkRunner runner)
    {
        if (!runner.IsServer)
            return;

        if (!runner.TryGetPlayerObject(
                playerRef,
                out var playerObject))
        {
            return;
        }

        runner.Despawn(playerObject);
    }
}
