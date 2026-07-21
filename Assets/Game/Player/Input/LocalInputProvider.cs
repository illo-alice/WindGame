using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class LocalInputProvider : MonoBehaviour
{
    private InputSystem_Actions _inputSystem;
    
    private Vector2 _moveValue;
    private Vector2 _lookValue;
    private bool _jumpPressed;

    public Vector3 AimTarget =>
        _cameraService.AimTarget;
    
    public bool Fire => _inputSystem.Player.Fire.IsPressed();
    
    public Vector2 Move
    {
        get
        {
            if (!_cameraService.TryGetTarget(out var cameraTarget)) return _moveValue;

            var forward = cameraTarget.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = cameraTarget.right;
            right.y = 0f;
            right.Normalize();

            var direction =
                forward * _moveValue.y +
                right * _moveValue.x;

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            return new Vector2(direction.x, direction.z);
        }
    }
    public Vector2 Look => _lookValue;
    public bool Jump
    {
        get
        {
            bool value = _jumpPressed;
            _jumpPressed = false;
            return value;
        }
    }
    public bool Sprint => _inputSystem.Player.Sprint.IsPressed();

    private CameraService _cameraService;
    
    [Inject]
    public void Construct(CameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public void OnEnable()
    {
        _inputSystem ??= new InputSystem_Actions();
        _inputSystem.Enable();
        
        _inputSystem.Player.Move.Enable();
        _inputSystem.Player.Look.Enable();
        _inputSystem.Player.Jump.Enable();
        _inputSystem.Player.Sprint.Enable();
        
        _inputSystem.Player.Move.performed += OnMove;
        _inputSystem.Player.Move.canceled += OnMove;

        _inputSystem.Player.Look.performed += OnLook;
        _inputSystem.Player.Look.canceled += OnLook;

        _inputSystem.Player.Jump.performed += OnJump;
    }
    
    public void OnDisable()
    {
        _inputSystem.Disable();
        
        _inputSystem.Player.Move.Disable();
        _inputSystem.Player.Look.Disable();
        _inputSystem.Player.Jump.Disable();
        _inputSystem.Player.Sprint.Disable();
        
        _inputSystem.Player.Move.performed -= OnMove;
        _inputSystem.Player.Move.canceled -= OnMove;

        _inputSystem.Player.Look.performed -= OnLook;
        _inputSystem.Player.Look.canceled -= OnLook;

        _inputSystem.Player.Jump.performed -= OnJump;
    }

    private void OnJump(InputAction.CallbackContext _)
    {
        _jumpPressed = true;
    }

    private void OnLook(InputAction.CallbackContext look)
    {
        _lookValue = look.ReadValue<Vector2>();
    }

    private void OnMove(InputAction.CallbackContext move)
    {
        _moveValue = move.ReadValue<Vector2>();
    }
}
