using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(LocalInputProvider))]
public sealed class LocalInputDeviceMonitor : MonoBehaviour
{
    private PlayerInput _playerInput;
    private LocalInputProvider _inputProvider;
    private LocalInputRegistry _registry;
    private bool _subscribed;

    public void Initialize(
        LocalInputRegistry registry,
        LocalInputProvider inputProvider)
    {
        _registry = registry;
        _inputProvider = inputProvider;
        _playerInput = GetComponent<PlayerInput>();

        Subscribe();

        var lostDeviceNames = GetLostDeviceNames(_playerInput);
        SetConnected(
            lostDeviceNames.Length == 0 &&
            !_playerInput.hasMissingRequiredDevices,
            lostDeviceNames
        );
    }

    private void OnEnable()
    {
        if (_registry != null)
            Subscribe();
    }

    private void OnDisable()
    {
        if (!_subscribed)
            return;

        _playerInput.onDeviceLost -= OnDeviceLost;
        _playerInput.onDeviceRegained -= OnDeviceRegained;
        _subscribed = false;
    }

    private void Subscribe()
    {
        if (_subscribed)
            return;

        _playerInput.onDeviceLost += OnDeviceLost;
        _playerInput.onDeviceRegained += OnDeviceRegained;
        _subscribed = true;
    }

    private void OnDeviceLost(PlayerInput playerInput)
    {
        SetConnected(false, GetLostDeviceNames(playerInput));
    }

    private void OnDeviceRegained(PlayerInput playerInput)
    {
        var lostDeviceNames = GetLostDeviceNames(playerInput);
        SetConnected(
            lostDeviceNames.Length == 0 &&
            playerInput.user.valid,
            lostDeviceNames
        );
    }

    private void SetConnected(bool connected, string[] deviceNames)
    {
        _inputProvider.SetInputEnabled(connected);
        _registry.NotifyDeviceConnectionChanged(
            _playerInput.playerIndex,
            connected,
            deviceNames
        );
    }

    private static string[] GetLostDeviceNames(PlayerInput playerInput)
    {
        var lostDevices = playerInput.user.lostDevices;
        var names = new string[lostDevices.Count];

        for (var i = 0; i < lostDevices.Count; i++)
        {
            var device = lostDevices[i];
            names[i] = string.IsNullOrWhiteSpace(device.displayName)
                ? device.name
                : device.displayName;
        }

        return names;
    }
}
