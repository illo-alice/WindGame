using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public sealed class LocalPlayerSpawnProvider :
    IPlayerSpawnerProvider,
    IDisposable
{
    private const float SpawnSpacing = 2f;

    private readonly LocalInputRegistry _inputs;

    private readonly Dictionary<PlayerRef, SpawnContext> _contexts = new();
    private readonly Dictionary<PlayerRef, NetworkObject[]> _players = new();

    public LocalPlayerSpawnProvider(LocalInputRegistry inputs)
    {
        _inputs = inputs;

        _inputs.ProviderRegistered += OnProviderRegistered;
        _inputs.ProviderUnregistered += OnProviderUnregistered;
    }

    public void Spawn(
        PlayerRef playerRef,
        NetworkRunner runner,
        NetworkObject prefab,
        Transform spawnPoint)
    {
        if (!runner.IsServer)
            return;

        _contexts[playerRef] = new SpawnContext(
            runner,
            prefab,
            spawnPoint
        );

        if (!_players.ContainsKey(playerRef))
            _players[playerRef] = new NetworkObject[LocalInputRegistry.Capacity];

        // Если input появился раньше, чем Fusion PlayerJoined.
        for (var slot = 0; slot < LocalInputRegistry.Capacity; slot++)
        {
            if (_inputs.TryGet(slot, out _))
                SpawnSlot(playerRef, (byte)slot);
        }
    }

    private void OnProviderRegistered(int playerIndex)
    {
        foreach (var playerRef in _contexts.Keys)
            SpawnSlot(playerRef, (byte)playerIndex);
    }

    private void SpawnSlot(PlayerRef playerRef, byte slot)
    {
        if (!_contexts.TryGetValue(playerRef, out var context))
            return;

        var players = _players[playerRef];

        if (players[slot] != null)
            return;

        var direction = slot == 0
            ? Vector3.left
            : Vector3.right;

        var position =
            context.SpawnPoint.position +
            direction * SpawnSpacing * 0.5f;

        var playerObject = context.Runner.Spawn(
            prefab: context.Prefab,
            position: position,
            rotation: Quaternion.identity,
            inputAuthority: playerRef,
            onBeforeSpawned: (_, networkObject) =>
            {
                networkObject
                    .GetComponent<LocalPlayerSlot>()
                    .SetIndex(slot);
            }
        );
        players[slot] = playerObject;

        if (slot == 0)
        {
            context.Runner.SetPlayerObject(
                playerRef,
                playerObject
            );
        }
    }

    private void OnProviderUnregistered(int playerIndex)
    {
        foreach (var playerRef in _contexts.Keys)
            DespawnSlot(playerRef, (byte)playerIndex);
    }

    private void DespawnSlot(PlayerRef playerRef, byte slot)
    {
        if (!_contexts.TryGetValue(playerRef, out var context) ||
            !_players.TryGetValue(playerRef, out var players))
            return;

        var playerObject = players[slot];

        if (playerObject == null)
            return;

        context.Runner.Despawn(playerObject);
        players[slot] = null;
    }

    public void Despawn(
        PlayerRef playerRef,
        NetworkRunner runner)
    {
        _contexts.Remove(playerRef);

        if (!_players.Remove(playerRef, out var players))
            return;

        foreach (var playerObject in players)
        {
            if (playerObject != null)
                runner.Despawn(playerObject);
        }
    }

    public void Dispose()
    {
        _inputs.ProviderRegistered -= OnProviderRegistered;
        _inputs.ProviderUnregistered -= OnProviderUnregistered;
    }

    private readonly struct SpawnContext
    {
        public readonly NetworkRunner Runner;
        public readonly NetworkObject Prefab;
        public readonly Transform SpawnPoint;

        public SpawnContext(
            NetworkRunner runner,
            NetworkObject prefab,
            Transform spawnPoint)
        {
            Runner = runner;
            Prefab = prefab;
            SpawnPoint = spawnPoint;
        }
    }
}
