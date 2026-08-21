using Fusion;
using UnityEngine;
using VContainer;

public class CameraFollowSetup : NetworkBehaviour
{
    [SerializeField] private Transform _target;

    private CameraService _cameraService;
    private LocalInputRegistry _localInputs;
    private InputSlot _inputSlot;
    private bool _hasLocalInputAuthority;

    [Inject]
    public void Construct(
        CameraService cameraService,
        LocalInputRegistry localInputs)
    {
        _cameraService = cameraService;
        _localInputs = localInputs;
    }

    public override void Spawned()
    {
        _cameraService.AddTarget(_target);

        _hasLocalInputAuthority = HasInputAuthority;

        if (!_hasLocalInputAuthority)
            return;

        _inputSlot = GetComponentInParent<InputSlot>();

        if (_localInputs.TryGet(_inputSlot.Index, out var localInput))
            localInput.SetAimOrigin(_target);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _cameraService.RemoveTarget(_target);

        if (!_hasLocalInputAuthority)
            return;

        if (_inputSlot != null &&
            _localInputs.TryGet(_inputSlot.Index, out var localInput))
        {
            localInput.ClearAimOrigin(_target);
        }
    }
}
