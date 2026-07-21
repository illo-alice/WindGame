using UnityEngine;
using VContainer;

public class Main : MonoBehaviour
{
    private Connection _connection;

    [Inject]
    public void Construct(Connection connection)
    {
        _connection = connection;
    }
    
    private async void Start()
    {
        await _connection.Connect();
        // -> spawn players
    }
}
