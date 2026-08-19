using Fusion;
using UnityEngine;
using VContainer;

public sealed class Spawn :
    SimulationBehaviour,
    IPlayerJoined,
    IPlayerLeft
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform _spawnPoint;

    private IPlayerSpawnerProvider _provider;

    [Inject]
    public void Construct(IPlayerSpawnerProvider provider)
    {
        _provider = provider;
    }

    public void PlayerJoined(PlayerRef playerRef)
    {
        _provider.Spawn(
            playerRef,
            Runner,
            _playerPrefab,
            _spawnPoint
        );
    }

    public void PlayerLeft(PlayerRef playerRef)
    {
        _provider.Despawn(
            playerRef,
            Runner
        );
    }
}
