using Unity.Cinemachine;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineThirdPersonAim _aim;
    
    public void SetFollow(Transform target)
    {
        _camera.Target.TrackingTarget = target;
    }
    
    public bool TryGetTarget(out Transform target)
    {
        if (_camera.Target.TrackingTarget != null)
        {
            target = _camera.Target.TrackingTarget;
            return true;
        }
        
        target = null;
        return false;
    }

    public Vector3 AimTarget => _aim.AimTarget;
}
