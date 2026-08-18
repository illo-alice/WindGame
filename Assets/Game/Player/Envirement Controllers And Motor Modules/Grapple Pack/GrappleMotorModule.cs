using UnityEngine;

public sealed class GrappleMotorModule : IMotorModule
{
    private readonly GrappleController _grappleController;
    private readonly float _swingAcceleration;
    private readonly float _stiffness;
    private readonly float _damping;
    private readonly float _maximumAcceleration;

    public GrappleMotorModule(
        GrappleController grappleController,
        float swingAcceleration,
        float stiffness,
        float damping,
        float maximumAcceleration)
    {
        _grappleController = grappleController;
        _swingAcceleration = swingAcceleration;
        _stiffness = stiffness;
        _damping = damping;
        _maximumAcceleration = maximumAcceleration;
    }

    public ForceData Evaluate(in MotorContext context)
    {
        if (!_grappleController.TryGetCurrentAnchor(out var anchor))
            return ForceData.None;

        var toAnchor =
            anchor.TargetPosition - context.PlayerTransform.position;
        var distance = toAnchor.magnitude;

        if (distance <= 0.0001f)
            return ForceData.None;

        var direction = toAnchor / distance;
        var acceleration = EvaluateRopeAcceleration(in context, direction, distance);
        var swingAcceleration = EvaluateSwingAcceleration(in context, direction);
        var totalAcceleration = direction * acceleration + swingAcceleration;

        if (totalAcceleration.sqrMagnitude <= 0.000001f)
            return ForceData.None;

        return new ForceData(totalAcceleration, ForceMode.Acceleration);
    }

    private float EvaluateRopeAcceleration(
        in MotorContext context,
        Vector3 direction,
        float distance)
    {
        var stretch = distance - _grappleController.RopeLength;

        if (stretch <= 0f)
            return 0f;

        var radialSpeed = Vector3.Dot(
            context.Physics.LinearVelocity,
            direction
        );

        return Mathf.Clamp(
            stretch * _stiffness - radialSpeed * _damping,
            0f,
            _maximumAcceleration
        );
    }

    private Vector3 EvaluateSwingAcceleration(
        in MotorContext context,
        Vector3 ropeDirection)
    {
        var moveDirection = new Vector3(
            context.Input.move.x,
            0f,
            context.Input.move.y
        );

        var tangentDirection = Vector3.ProjectOnPlane(
            moveDirection,
            ropeDirection
        );

        if (tangentDirection.sqrMagnitude <= 0.0001f)
            return Vector3.zero;

        return tangentDirection.normalized *
               _swingAcceleration *
               Mathf.Clamp01(moveDirection.magnitude);
    }
}
