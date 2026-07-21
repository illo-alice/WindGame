using Fusion;
using UnityEngine;

public class Spawn :
    SimulationBehaviour,
    IPlayerJoined,
    IPlayerLeft
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform _spawnPoint;
    
    public void PlayerJoined(PlayerRef playerRef)
    {
        if (!Runner.IsServer) return;

        var playerObject = Runner.Spawn(
                prefab: _playerPrefab,
                inputAuthority: playerRef,
                position: _spawnPoint.position
            );
        
        Runner.SetPlayerObject(playerRef, playerObject);
    }

    public void PlayerLeft(PlayerRef playerRef)
    {
        if (!Runner.IsServer) return;
        
        if (Runner.TryGetPlayerObject(playerRef, out var playerObject))
        {
            Runner.Despawn(playerObject);
        }
    }
}
