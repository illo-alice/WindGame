using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using VContainer;

public class FusionInputProvider : MonoBehaviour, INetworkRunnerCallbacks
{
    private readonly NetworkButtons[] _buttons =
        new NetworkButtons[LocalInputRegistry.Capacity];

    private LocalInputRegistry _localInputs;

    [Inject]
    public void Construct(LocalInputRegistry localInputs)
    {
        _localInputs = localInputs;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(new InputData
        {
            player0 = BuildPlayerInput(0),
            player1 = BuildPlayerInput(1),
        });
    }

    private PlayerInputData BuildPlayerInput(int playerIndex)
    {
        if (_localInputs == null ||
            !_localInputs.TryGet(playerIndex, out var provider))
        {
            _buttons[playerIndex] = default;
            return default;
        }

        ref var buttons = ref _buttons[playerIndex];
        buttons.Set(InputType.Jump, provider.Jump);
        buttons.Set(InputType.Sprint, provider.Sprint);
        buttons.Set(InputType.Fire, provider.Fire);

        return new PlayerInputData
        {
            buttons = buttons,
            move = provider.Move,
            aimTarget = provider.AimTarget,
        };
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        input.Set(default(InputData));
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
