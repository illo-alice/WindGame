using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkObject))]
public sealed class GrappleAnchor : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private bool _active = true;
    [SerializeField, Min(0f)] private float _captureRange = 25f;
    [SerializeField, Range(0f, 90f)] private float _aimAssistAngle = 20f;

    private NetworkObject _networkObject;

    public bool IsActive => _active;
    public float CaptureRange => _captureRange;
    public float AimAssistAngle => _aimAssistAngle;
    public Vector3 TargetPosition => transform.position;
    public NetworkObject NetworkObject => _networkObject;

    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;

        var angleStep = 360f / 32f;
        Vector3 lastPoint = new Vector3(_captureRange, 0, 0);

        for (var i = 1; i <= 32; i++)
        {
            var angle = i * angleStep * Mathf.Deg2Rad;
            var currentPoint = new Vector3(Mathf.Cos(angle) * _captureRange, Mathf.Sin(angle) * _captureRange, 0);
        
            Gizmos.DrawLine(lastPoint, currentPoint);
            lastPoint = currentPoint;
        }
            
    }
}
