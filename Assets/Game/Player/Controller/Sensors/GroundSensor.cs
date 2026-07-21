using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public sealed class GroundSensor : MonoBehaviour, IReadOnlyGroundSensor
{
    private const int HitCapacity = 8;

    [SerializeField] private CapsuleCollider _capsule;
    [SerializeField] private LayerMask _groundMask = ~0;
    [SerializeField, Min(0.01f)] private float _probeDistance = 0.15f;
    [SerializeField, Range(0.1f, 1f)] private float _radiusScale = 0.9f;
    [SerializeField, Range(0f, 89f)] private float _maxGroundAngle = 60f;
    [SerializeField] private QueryTriggerInteraction _triggerInteraction =
        QueryTriggerInteraction.Ignore;

    private readonly RaycastHit[] _hits = new RaycastHit[HitCapacity];

    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public Collider GroundCollider { get; private set; }

    public void Scan()
    {
        ResetResult();

        if (_capsule == null || !_capsule.enabled)
            return;

        Transform capsuleTransform = _capsule.transform;
        Vector3 up = capsuleTransform.up;
        Vector3 scale = capsuleTransform.lossyScale;

        float radiusScale = Mathf.Max(
            Mathf.Abs(scale.x),
            Mathf.Abs(scale.z)
        );
        float radius = _capsule.radius * radiusScale;
        float height = _capsule.height * Mathf.Abs(scale.y);
        float halfHeight = Mathf.Max(height * 0.5f, radius);
        float probeRadius = radius * _radiusScale;
        float skin = radius - probeRadius;

        Vector3 center = capsuleTransform.TransformPoint(_capsule.center);
        Vector3 bottomSphereCenter = center - up * (halfHeight - radius);
        float castDistance = skin + _probeDistance;

        int hitCount = Physics.SphereCastNonAlloc(
            bottomSphereCenter,
            probeRadius,
            -up,
            _hits,
            castDistance,
            _groundMask,
            _triggerInteraction
        );

        float minimumGroundDot = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
        float nearestDistance = float.PositiveInfinity;
        Rigidbody ownBody = _capsule.attachedRigidbody;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = _hits[i];
            Collider hitCollider = hit.collider;

            if (hitCollider == null ||
                hitCollider == _capsule ||
                ownBody != null && hitCollider.attachedRigidbody == ownBody)
            {
                continue;
            }

            if (Vector3.Dot(hit.normal, up) < minimumGroundDot)
                continue;

            if (hit.distance >= nearestDistance)
                continue;

            nearestDistance = hit.distance;
            GroundCollider = hitCollider;
            GroundNormal = hit.normal;
            IsGrounded = true;
        }
    }

    private void ResetResult()
    {
        IsGrounded = false;
        GroundNormal = transform.up;
        GroundCollider = null;
    }

    private void OnValidate()
    {
        if (_capsule == null)
            _capsule = GetComponent<CapsuleCollider>();
    }

    private void OnDrawGizmosSelected()
    {
        if (_capsule == null)
            return;

        Transform capsuleTransform = _capsule.transform;
        Vector3 scale = capsuleTransform.lossyScale;
        float radius = _capsule.radius * Mathf.Max(
            Mathf.Abs(scale.x),
            Mathf.Abs(scale.z)
        );
        float height = _capsule.height * Mathf.Abs(scale.y);
        float halfHeight = Mathf.Max(height * 0.5f, radius);
        float probeRadius = radius * _radiusScale;
        Vector3 up = capsuleTransform.up;
        Vector3 center = capsuleTransform.TransformPoint(_capsule.center);
        Vector3 bottomSphereCenter = center - up * (halfHeight - radius);
        Vector3 end = bottomSphereCenter - up * ((radius - probeRadius) + _probeDistance);

        Gizmos.color = IsGrounded ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(end, probeRadius);
        Gizmos.DrawLine(bottomSphereCenter, end);
    }
}
