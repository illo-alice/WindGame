using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public sealed class LocalInputProvider : MonoBehaviour
{
    private PlayerInput _playerInput;
    private LocalInputRegistry _registry;
    private CameraService _cameraService;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _fireAction;

    private bool _jumpPressed;
    private bool _initialized;
    private bool _inputEnabled = true;
    private Transform _aimOrigin;

    public int PlayerIndex => _playerInput != null ? _playerInput.playerIndex : -1;
    public Vector2 Look => _inputEnabled
        ? _lookAction?.ReadValue<Vector2>() ?? default
        : default;
    public bool Sprint => _inputEnabled && _sprintAction?.IsPressed() == true;
    public bool Fire => _inputEnabled && _fireAction?.IsPressed() == true;

    public Vector2 Aim
    {
        get
        {
            if (!_inputEnabled)
                return default;

            if (TryGetPointerPosition(out var screenPosition) &&
                _aimOrigin != null &&
                _cameraService.TryGetAimDirection(
                    screenPosition,
                    _aimOrigin.position,
                    out var pointerDirection))
            {
                return pointerDirection;
            }

            var direction = Look;

            return direction.sqrMagnitude > 0.01f
                ? direction.normalized
                : default;
        }
    }

    public Vector2 Move
    {
        get
        {
            if (!_inputEnabled)
                return default;

            return _moveAction?.ReadValue<Vector2>() ?? default;
        }
    }

    public bool Jump
    {
        get
        {
            if (!_inputEnabled)
            {
                _jumpPressed = false;
                return false;
            }

            var value = _jumpPressed;
            _jumpPressed = false;
            return value;
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        _inputEnabled = enabled;

        if (!enabled)
            _jumpPressed = false;
    }

    public void SetAimOrigin(Transform origin)
    {
        _aimOrigin = origin;
    }

    public void ClearAimOrigin(Transform origin)
    {
        if (_aimOrigin == origin)
            _aimOrigin = null;
    }

    public void Initialize(
        LocalInputRegistry registry,
        CameraService cameraService)
    {
        _playerInput = GetComponent<PlayerInput>();
        _registry = registry;
        _cameraService = cameraService;

        var actions = _playerInput.actions;
        _moveAction = actions.FindAction("Player/Move", true);
        _lookAction = actions.FindAction("Player/Look", true);
        _jumpAction = actions.FindAction("Player/Jump", true);
        _sprintAction = actions.FindAction("Player/Sprint", true);
        _fireAction = actions.FindAction("Player/Fire", true);

        Activate();
    }

    private void OnEnable()
    {
        if (_registry != null)
            Activate();
    }

    private void Activate()
    {
        if (_initialized)
            return;

        _jumpAction.performed += OnJump;
        _registry.Register(_playerInput.playerIndex, this);
        _initialized = true;
    }

    private void OnDisable()
    {
        if (!_initialized)
            return;

        _jumpAction.performed -= OnJump;
        _registry.Unregister(_playerInput.playerIndex, this);
        _initialized = false;
        _jumpPressed = false;
    }

    private void OnJump(InputAction.CallbackContext _)
    {
        _jumpPressed = true;
    }

    private bool TryGetPointerPosition(out Vector2 position)
    {
        foreach (var device in _playerInput.devices)
        {
            if (device is Pointer pointer)
            {
                position = pointer.position.ReadValue();
                return true;
            }
        }

        position = default;
        return false;
    }
}
