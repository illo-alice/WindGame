using Cysharp.Threading.Tasks;
using Fusion;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public sealed class Connection
{
    private readonly NetworkRunner _runner;
    private readonly VContainerNetworkObjectProvider _objectProvider;

    public Connection(NetworkRunner runner, VContainerNetworkObjectProvider objectProvider)
    {
        _runner = runner;
        _objectProvider = objectProvider;
    }

    public async UniTask<StartGameResult> Connect()
    {
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        _runner.ProvideInput = true;
        
        var realtimeClient = new RealtimeClient();
        realtimeClient.ProtocolPorts.SetUdpDefaultOld();
        
        return await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "WindGameRoom",
            Scene = scene,
            SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ObjectProvider = _objectProvider,
            RealtimeClient = realtimeClient,
        });
    }
}
