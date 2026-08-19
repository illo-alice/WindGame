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

    public int PlayerIndex => _playerInput != null ? _playerInput.playerIndex : -1;
    public Vector2 Look => _inputEnabled
        ? _lookAction?.ReadValue<Vector2>() ?? default
        : default;
    public bool Sprint => _inputEnabled && _sprintAction?.IsPressed() == true;
    public bool Fire => _inputEnabled && _fireAction?.IsPressed() == true;
    public Vector3 AimTarget => _inputEnabled && _cameraService != null
        ? _cameraService.AimTarget
        : default;

    public Vector2 Move
    {
        get
        {
            if (!_inputEnabled)
                return default;

            var moveValue = _moveAction?.ReadValue<Vector2>() ?? default;

            if (_cameraService == null ||
                !_cameraService.TryGetTarget(out var cameraTarget))
                return moveValue;

            var forward = cameraTarget.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = cameraTarget.right;
            right.y = 0f;
            right.Normalize();

            var direction = forward * moveValue.y + right * moveValue.x;

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            return new Vector2(direction.x, direction.z);
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
}
