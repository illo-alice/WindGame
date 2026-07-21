using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionInputProvider : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private LocalInputProvider _localInputProvider;
    
    private NetworkButtons _buttons;
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        SetNetworkButtons();
        
        input.Set(
                new InputData
                {
                    buttons = _buttons,
                    move = _localInputProvider.Move,
                    aimTarget = _localInputProvider.AimTarget,
                }
            );
    }

    private void SetNetworkButtons()
    {
        _buttons.Set(InputType.Jump, _localInputProvider.Jump);
        _buttons.Set(InputType.Sprint, _localInputProvider.Sprint);
        _buttons.Set(InputType.Fire, _localInputProvider.Fire);
    }
    
    // useless below
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        input.Set(new InputData());
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
}
