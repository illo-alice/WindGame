using Unity.Cinemachine;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineTargetGroup _cameraTargetGroup;

    private Camera _outputCamera;

    public void AddTarget(Transform target)
    {
        if (_cameraTargetGroup.FindMember(_cameraTargetGroup.transform) >= 0)
            _cameraTargetGroup.RemoveMember(_cameraTargetGroup.transform);

        if (_cameraTargetGroup.FindMember(target) < 0)
            _cameraTargetGroup.AddMember(target, 1, 3);
    }

    public void RemoveTarget(Transform target)
    {
        _cameraTargetGroup.RemoveMember(target);
    }

    public bool TryGetAimDirection(
        Vector2 screenPosition,
        Vector3 origin,
        out Vector2 direction)
    {
        var outputCamera = GetOutputCamera();

        if (outputCamera == null)
        {
            direction = default;
            return false;
        }

        var ray = outputCamera.ScreenPointToRay(screenPosition);
        var gameplayPlane = new Plane(Vector3.forward, origin);

        if (!gameplayPlane.Raycast(ray, out var distance))
        {
            direction = default;
            return false;
        }

        var worldPosition = ray.GetPoint(distance);
        direction = new Vector2(
            worldPosition.x - origin.x,
            worldPosition.y - origin.y
        );

        if (direction.sqrMagnitude <= 0.0001f)
        {
            direction = default;
            return false;
        }

        direction.Normalize();
        return true;
    }

    private Camera GetOutputCamera()
    {
        if (_outputCamera != null)
            return _outputCamera;

        var brain = CinemachineCore.FindPotentialTargetBrain(_camera);
        _outputCamera = brain != null ? brain.OutputCamera : null;
        return _outputCamera;
    }
}
