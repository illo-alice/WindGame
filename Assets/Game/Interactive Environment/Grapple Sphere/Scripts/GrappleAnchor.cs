using System.Collections.Generic;
using UnityEngine;

public sealed class GrappleAnchor : MonoBehaviour
{
    private static readonly List<GrappleAnchor> _anchors = new();

    [Header("Targeting")]
    [SerializeField] private bool _active = true;
    [SerializeField, Min(0f)] private float _captureRange = 25f;
    [SerializeField, Range(0f, 90f)] private float _aimAssistAngle = 20f;

    public static IReadOnlyList<GrappleAnchor> All => _anchors;

    public bool IsActive => _active;
    public float CaptureRange => _captureRange;
    public float AimAssistAngle => _aimAssistAngle;
    public Vector3 TargetPosition => transform.position;

    private void OnEnable()
    {
        if (!_anchors.Contains(this))
            _anchors.Add(this);
    }

    private void OnDisable()
    {
        _anchors.Remove(this);
    }
}