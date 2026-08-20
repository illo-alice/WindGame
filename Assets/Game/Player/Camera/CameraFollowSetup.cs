using Fusion;
using UnityEngine;
using VContainer;

public class CameraFollowSetup : NetworkBehaviour
{
    [SerializeField] private Transform _target;

    private CameraService _cameraService;
    private LocalInputRegistry _localInputs;
    private LocalPlayerSlot _localPlayerSlot;

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
        if (!HasInputAuthority)
            return;

        _localPlayerSlot = GetComponentInParent<LocalPlayerSlot>();
        _cameraService.AddTarget(_target);

        if (_localInputs.TryGet(_localPlayerSlot.Index, out var localInput))
            localInput.SetAimOrigin(_target);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (!HasInputAuthority)
            return;

        _cameraService.RemoveTarget(_target);

        if (_localPlayerSlot != null &&
            _localInputs.TryGet(_localPlayerSlot.Index, out var localInput))
        {
            localInput.ClearAimOrigin(_target);
        }
    }
}
