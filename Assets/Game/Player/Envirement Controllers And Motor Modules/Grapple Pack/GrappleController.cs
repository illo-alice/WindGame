using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GrappleController : NetworkBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Transform _origin;
    [SerializeField, Min(0f)] private float _radius = 30f;
    [SerializeField] private LayerMask _actionEnvirementLayerMask;

    [Header("Rope")]
    [SerializeField, Min(0f)] private float _initialShortening = 2f;
    [SerializeField, Min(0.1f)] private float _minimumRopeLength = 2f;

    private readonly ColliderScanner _scanner = new();

    [Networked] public NetworkObject CurrentAnchor { get; private set; }
    [Networked] public float RopeLength { get; private set; }

    public bool IsAttached => CurrentAnchor != null;

    public void Simulate(in InputData input)
    {
        if (!input.buttons.IsSet(InputType.Fire))
        {
            Detach();
            return;
        }

        if (TryResolveCurrentAnchor(out _))
            return;

        Detach();

        var aimVector = input.aimTarget - _origin.position;

        if (aimVector.sqrMagnitude < 0.0001f)
            return;

        if (!TryGetBestAnchor(aimVector.normalized, out var anchor))
            return;

        Attach(anchor);
    }

    private void Attach(GrappleAnchor anchor)
    {
        CurrentAnchor = anchor.NetworkObject;

        var distance = Vector3.Distance(
            _origin.position,
            anchor.TargetPosition
        );

        RopeLength = Mathf.Max(
            _minimumRopeLength,
            distance - _initialShortening
        );
    }

    public void Detach()
    {
        CurrentAnchor = default;
        RopeLength = 0f;
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
        out GrappleAnchor bestAnchor)
    {
        var length = _scanner.Scan(
            _origin.position,
            _radius,
            _actionEnvirementLayerMask
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

            var distance = toAnchor.magnitude;

            if (distance > anchor.CaptureRange)
                continue;

            var angle = Vector3.Angle(
                aimDirection,
                toAnchor.normalized
            );

            if (angle > anchor.AimAssistAngle)
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
