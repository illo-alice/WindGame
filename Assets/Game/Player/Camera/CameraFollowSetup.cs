using Fusion;
using UnityEngine;
using VContainer;

public class CameraFollowSetup : NetworkBehaviour
{
    [SerializeField] private Transform _target;
    
    private CameraService _cameraService;

    [Inject]
    public void Construct(CameraService cameraService)
    {
        _cameraService = cameraService;
    }
    
    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            _cameraService.SetFollow(_target);
        }
    }
}
