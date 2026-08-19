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
        RefreshConnectionState();
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

    private void OnDeviceLost(PlayerInput _)
    {
        RefreshConnectionState();
    }

    private void OnDeviceRegained(PlayerInput _)
    {
        RefreshConnectionState();
    }

    private void RefreshConnectionState()
    {
        SetConnected(!_playerInput.hasMissingRequiredDevices);
    }

    private void SetConnected(bool connected)
    {
        _inputProvider.SetInputEnabled(connected);
        _registry.NotifyDeviceConnectionChanged(
            _playerInput.playerIndex,
            connected
        );
    }
}
