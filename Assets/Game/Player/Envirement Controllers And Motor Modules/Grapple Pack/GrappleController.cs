using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GrappleController : NetworkBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform _origin;
    [SerializeField, Min(0f)] private float _radius = 30f;
    [SerializeField] private LayerMask _actionEnvirementLayerMask;
    [SerializeField, Range(0f, 180f)] private float _automaticAimAngle = 90f;
    [SerializeField, Range(0f, 1f)] private float _automaticHorizontalBias = 0.5f;

    [Header("Rope")]
    [SerializeField, Min(0f)] private float _initialShortening = 2f;
    [SerializeField, Min(0.1f)] private float _minimumRopeLength = 2f;

    private readonly ColliderScanner _scanner = new();

    [Networked] public NetworkObject CurrentAnchor { get; private set; }
    [Networked] public float RopeLength { get; private set; }
    [Networked] public float SpringLength { get; private set; }

    public bool IsAttached => CurrentAnchor != null;
    public Vector3 OriginPosition => _origin.position;

    public void Simulate(in PlayerInputData input)
    {
        if (!input.buttons.IsSet(InputType.Fire))
        {
            Detach();
            return;
        }

        if (TryResolveCurrentAnchor(out _))
            return;

        Detach();

        var aimVector = new Vector3(
            input.aim.x,
            input.aim.y,
            0f
        );

        var hasManualAim = aimVector.sqrMagnitude >= 0.01f;

        if (!hasManualAim)
        {
            var horizontalDirection = Mathf.Abs(input.move.x) >= 0.1f
                ? Mathf.Sign(input.move.x)
                : 0f;

            aimVector = new Vector3(
                horizontalDirection * _automaticHorizontalBias,
                1f,
                0f
            );
        }

        if (!TryGetBestAnchor(
                aimVector.normalized,
                hasManualAim,
                out var anchor))
            return;

        Attach(anchor);
    }

    private void Attach(GrappleAnchor anchor)
    {
        CurrentAnchor = anchor.NetworkObject;

        var toAnchor = anchor.TargetPosition - _origin.position;
        toAnchor.z = 0f;
        var distance = toAnchor.magnitude;

        RopeLength = Mathf.Max(
            _minimumRopeLength,
            distance
        );

        SpringLength = Mathf.Max(
            _minimumRopeLength,
            distance - _initialShortening
        );
    }

    public void Detach()
    {
        CurrentAnchor = default;
        RopeLength = 0f;
        SpringLength = 0f;
    }

    public bool TryGetCurrentAnchor(out GrappleAnchor anchor)
    {
        return TryResolveCurrentAnchor(out anchor);
    }

    private bool TryResolveCurrentAnchor(out GrappleAnchor anchor)
    {
        anchor = null;

        if (CurrentAnchor == null)
            return false;

        return CurrentAnchor.TryGetComponent(out anchor) && anchor.IsActive;
    }

    private bool TryGetBestAnchor(
        Vector3 aimDirection,
        bool hasManualAim,
        out GrappleAnchor bestAnchor)
    {
        var length = _scanner.Scan(
            _origin.position,
            _radius,
            _actionEnvirementLayerMask,
            QueryTriggerInteraction.Collide
        );

        var bestAngle = float.MaxValue;
        var bestDistance = float.MaxValue;
        bestAnchor = null;

        for (var i = 0; i < length; i++)
        {
            var collider = _scanner.GetResult(i);
            var anchor = collider.GetComponentInParent<GrappleAnchor>();

            if (anchor == null || !anchor.IsActive)
                continue;

            var toAnchor =
                anchor.TargetPosition - _origin.position;

            toAnchor.z = 0f;

            var distance = toAnchor.magnitude;

            if (distance > anchor.CaptureRange)
                continue;

            var angle = Vector3.Angle(
                aimDirection,
                toAnchor.normalized
            );

            var maximumAngle = hasManualAim
                ? anchor.AimAssistAngle
                : _automaticAimAngle;

            if (angle > maximumAngle)
                continue;

            if (angle < bestAngle || Mathf.Approximately(angle, bestAngle) && distance < bestDistance)
            {
                bestAngle = angle;
                bestDistance = distance;
                bestAnchor = anchor;
            }
        }

        return bestAnchor != null;
    }
}
