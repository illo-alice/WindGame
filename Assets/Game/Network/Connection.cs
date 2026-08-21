using System;
using Cysharp.Threading.Tasks;
using Fusion;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public sealed class Connection
{
    private readonly NetworkRunner _runner;
    private readonly VContainerNetworkObjectProvider _objectProvider;
    private readonly GameSessionSettings _sessionSettings;
    private readonly SteamService _steamService;

    public Connection(
        NetworkRunner runner,
        VContainerNetworkObjectProvider objectProvider,
        GameSessionSettings sessionSettings,
        SteamService steamService)
    {
        _runner = runner;
        _objectProvider = objectProvider;
        _sessionSettings = sessionSettings;
        _steamService = steamService;
    }

    public async UniTask<StartGameResult> Connect()
    {
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        _runner.ProvideInput = true;
        
        var gameMode = _sessionSettings.SpawnMode == PlayerSpawnMode.Local
            ? GameMode.Single
            : GameMode.AutoHostOrClient;

        var startGameArgs = new StartGameArgs
        {
            GameMode = gameMode,
            Scene = scene,
            SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ObjectProvider = _objectProvider,
        };

        if (!_steamService.IsInitialized)
        {
            throw new InvalidOperationException(
                "Steam must be initialized before connecting."
            );
        }

        startGameArgs.AuthValues = new AuthenticationValues
        {
            UserId = _steamService.SteamId
        };
        
        if (gameMode != GameMode.Single)
        {
            var realtimeClient = new RealtimeClient();
            realtimeClient.ProtocolPorts.SetUdpDefaultOld();

            startGameArgs.SessionName = "WindGameRoom";
            startGameArgs.RealtimeClient = realtimeClient;
        }

        return await _runner.StartGame(startGameArgs);
    }
}
