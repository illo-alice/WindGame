using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class Main : MonoBehaviour
{
    private Connection _connection;
    private GameSessionSettings _sessionSettings;

    [Inject]
    public void Construct(
        Connection connection,
        GameSessionSettings sessionSettings)
    {
        _connection = connection;
        _sessionSettings = sessionSettings;
    }

    private async UniTask Start()
    {
        try
        {
            var mode = PlayerSpawnMode.Local;

            _sessionSettings.SetSpawnMode(mode);

            var result = await _connection.Connect();

            if (!result.Ok)
                Debug.LogError($"Failed to start {mode} game: {result.ErrorMessage}");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
