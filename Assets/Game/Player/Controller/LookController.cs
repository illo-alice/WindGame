using Fusion;
using UnityEngine;
using VContainer;

public class LookController : NetworkBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _sensitivity = 0.15f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 70f;

    private LocalInputProvider _localInputProvider;
    
    private float _yaw;
    private float _pitch;

    [Inject]
    public void Construct(LocalInputProvider localInputProvider)
    {
        _localInputProvider = localInputProvider;
    }

    public override void Spawned()
    {
        enabled = HasInputAuthority;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public override void Render()
    {
        _yaw += _localInputProvider.Look.x * _sensitivity;
        _pitch -= _localInputProvider.Look.y * _sensitivity;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        _cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
}
