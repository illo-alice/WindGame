using System.Linq;
using Fusion;
using UnityEngine;

public sealed class OnlinePlayerSpawnProvider :
    IPlayerSpawnerProvider
{
    private readonly bool[] connected = new bool[2];
    
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
                    .GetComponent<InputSlot>()
                    .SetIndex(0);

                for (byte i = 0; i < connected.Length; i++)
                {
                    var connect = connected[i];
                    if (!connect)
                    {
                        networkObject.GetComponent<PlayerSlot>()
                            .SetIndex(i);
                    }
                }
            }
        );

        runner.SetPlayerObject(
            playerRef,
            playerObject
        );
        
        var steamId = runner.GetPlayerUserId(playerRef);
        var profileId = $"steam:{steamId}";

        Debug.Log(
            $"Player {playerRef} authenticated as {profileId}"
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
