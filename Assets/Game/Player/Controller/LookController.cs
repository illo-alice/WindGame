using Fusion;
using UnityEngine;
using VContainer;

public class LookController : NetworkBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _sensitivity = 0.15f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 70f;

    private LocalInputRegistry _localInputs;
    private LocalPlayerSlot _localPlayerSlot;
    
    private float _yaw;
    private float _pitch;

    [Inject]
    public void Construct(LocalInputRegistry localInputs)
    {
        _localInputs = localInputs;
    }

    public override void Spawned()
    {
        enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    public override void Render()
    {
        if (_localInputs == null ||
            _localPlayerSlot == null ||
            !_localInputs.TryGet(_localPlayerSlot.Index, out var localInput))
            return;

        _yaw += localInput.Look.x * _sensitivity;
        _pitch -= localInput.Look.y * _sensitivity;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        _cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
}
