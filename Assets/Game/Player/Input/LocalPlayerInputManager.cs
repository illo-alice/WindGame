using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInputManager))]
public sealed class LocalPlayerInputManager : MonoBehaviour
{
    private PlayerInputManager _playerInputManager;
    private LocalInputRegistry _registry;
    private CameraService _cameraService;

    [Inject]
    public void Construct(
        LocalInputRegistry registry,
        CameraService cameraService)
    {
        _registry = registry;
        _cameraService = cameraService;
        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += OnPlayerJoined;

        foreach (var playerInput in PlayerInput.all)
            TryInitialize(playerInput);
    }

    private void OnDestroy()
    {
        if (_playerInputManager != null)
            _playerInputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        TryInitialize(playerInput);
    }

    private void TryInitialize(PlayerInput playerInput)
    {
        if (playerInput.playerIndex >= LocalInputRegistry.Capacity)
            return;

        var inputProvider =
            playerInput.GetComponent<LocalInputProvider>();

        inputProvider.Initialize(_registry, _cameraService);

        playerInput.GetComponent<LocalInputDeviceMonitor>()
            .Initialize(_registry, inputProvider);
    }
}
