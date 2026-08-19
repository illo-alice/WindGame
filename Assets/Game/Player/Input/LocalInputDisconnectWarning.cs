using UnityEngine;
using UnityEngine.UI;
using VContainer;

[DisallowMultipleComponent]
public sealed class LocalInputDisconnectWarning : MonoBehaviour
{
    [SerializeField] private Image _image;

    private readonly bool[] _disconnected =
        new bool[LocalInputRegistry.Capacity];

    private LocalInputRegistry _registry;

    [Inject]
    public void Construct(LocalInputRegistry registry)
    {
        _registry = registry;
        _registry.DeviceConnectionChanged += OnDeviceConnectionChanged;
        _registry.ProviderUnregistered += OnProviderUnregistered;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_registry != null)
        {
            _registry.DeviceConnectionChanged -= OnDeviceConnectionChanged;
            _registry.ProviderUnregistered -= OnProviderUnregistered;
        }
    }

    private void OnDeviceConnectionChanged(
        int playerIndex,
        bool connected)
    {
        if ((uint)playerIndex >= LocalInputRegistry.Capacity)
            return;

        _disconnected[playerIndex] = !connected;
        Refresh();
    }

    private void OnProviderUnregistered(int playerIndex)
    {
        if ((uint)playerIndex >= LocalInputRegistry.Capacity)
            return;

        _disconnected[playerIndex] = false;
        Refresh();
    }

    private void Refresh()
    {
        var show = false;

        foreach (var disconnected in _disconnected)
            show |= disconnected;

        _image.gameObject.SetActive(show);
    }
}
