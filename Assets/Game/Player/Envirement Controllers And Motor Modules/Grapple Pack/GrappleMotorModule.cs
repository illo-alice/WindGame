using UnityEngine;

public sealed class GrappleMotorModule : IMotorModule, IMotorConstraint
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
            anchor.TargetPosition - _grappleController.OriginPosition;

        toAnchor.z = 0f;

        var distance = toAnchor.magnitude;

        if (distance <= 0.0001f)
            return ForceData.None;

        var direction = toAnchor / distance;
        var ropeAcceleration = EvaluateRopeAcceleration(
            in context,
            direction,
            distance
        );
        var swingAcceleration = EvaluateSwingAcceleration(in context, direction);
        var totalAcceleration =
            direction * ropeAcceleration + swingAcceleration;

        if (totalAcceleration.sqrMagnitude <= 0.000001f)
            return ForceData.None;

        return new ForceData(totalAcceleration, ForceMode.Acceleration);
    }

    public void ApplyConstraint(
        in MotorContext context,
        IPlayerPhysics physics)
    {
        if (!_grappleController.TryGetCurrentAnchor(out var anchor))
            return;

        var originPosition = _grappleController.OriginPosition;
        var fromAnchor = originPosition - anchor.TargetPosition;
        fromAnchor.z = 0f;

        var distance = fromAnchor.magnitude;

        if (distance <= 0.0001f)
            return;

        var ropeLength = _grappleController.RopeLength;
        var outwardDirection = fromAnchor / distance;
        
        if (distance > ropeLength)
        {
            var correctedOrigin =
                anchor.TargetPosition + outwardDirection * ropeLength;

            var correction = correctedOrigin - originPosition;
            correction.z = 0f;

            physics.Position += correction;
            distance = ropeLength;
        }

        var velocity = physics.LinearVelocity;
        var outwardSpeed = Vector3.Dot(velocity, outwardDirection);

        if (outwardSpeed <= 0f)
            return;

        // Максимальная допустимая скорость наружу, чтобы за следующий
        // физический тик не пересечь RopeLength.
        var remainingDistance = Mathf.Max(0f, ropeLength - distance);
        var maximumOutwardSpeed =
            remainingDistance / context.DeltaTime;

        if (outwardSpeed > maximumOutwardSpeed)
        {
            physics.LinearVelocity =
                velocity -
                outwardDirection *
                (outwardSpeed - maximumOutwardSpeed);
        }
    }

    private float EvaluateRopeAcceleration(
        in MotorContext context,
        Vector3 direction,
        float distance)
    {
        var stretch = distance - _grappleController.SpringLength;

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
        var horizontalInput = context.Input.move.x;

        if (Mathf.Abs(horizontalInput) <= 0.0001f)
            return Vector3.zero;

        var moveDirection = Vector3.right * horizontalInput;

        var tangentDirection = Vector3.ProjectOnPlane(
            moveDirection,
            ropeDirection
        );

        if (tangentDirection.sqrMagnitude <= 0.0001f)
            return Vector3.zero;

        return tangentDirection.normalized *
               _swingAcceleration *
               Mathf.Abs(horizontalInput);
    }
}
