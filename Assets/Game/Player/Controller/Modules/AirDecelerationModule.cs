using UnityEngine;

public sealed class AirDecelerationModule : IMotorModule
{
    private readonly float _deceleration;

    public AirDecelerationModule(float deceleration)
    {
        _deceleration = deceleration;
    }

    public ForceData Evaluate(in MotorContext context)
    {
        if (context.Input.move.sqrMagnitude > 0.0001f ||
            context.Ground.IsGrounded)
        {
            return ForceData.None;
        }

        var velocity = context.Physics.HorizontalVelocity;

        if (velocity.sqrMagnitude <= 0.0001f)
            return ForceData.None;

        var newVelocity = Vector3.MoveTowards(
            velocity,
            Vector3.zero,
            _deceleration * context.DeltaTime
        );

        return new ForceData(
            newVelocity - velocity,
            ForceMode.VelocityChange
        );
    }
}
