using UnityEngine;

public sealed class HorizontalModule : IMotorModule
{
    private readonly float _acceleration;
    private readonly float _maxSpeed;
    private readonly float _sprintMaxSpeed;
    private readonly float _sprintAccelerationMultiplier;

    public HorizontalModule(
        float acceleration,
        float maxSpeed,
        float sprintMaxSpeed,
        float sprintAccelerationMultiplier
    )
    {
        _acceleration = acceleration;
        _maxSpeed = maxSpeed;
        _sprintMaxSpeed = sprintMaxSpeed;
        _sprintAccelerationMultiplier = sprintAccelerationMultiplier;
    }

    public ForceData Evaluate(in MotorContext context)
    {
        Vector3 direction = new(
            context.Input.move.x,
            0f,
            context.Input.move.y
        );

        if (direction.sqrMagnitude <= 0.0001f)
            return ForceData.None;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        bool isSprinting = context.Input.buttons.IsSet(InputType.Sprint);
        float maxSpeed = isSprinting ? _sprintMaxSpeed : _maxSpeed;
        float acceleration = isSprinting
            ? _acceleration * _sprintAccelerationMultiplier
            : _acceleration;

        var currentVelocity = context.Physics.HorizontalVelocity;
        var targetVelocity = direction * maxSpeed;
        var newVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            acceleration * context.DeltaTime
        );

        return new ForceData(
            newVelocity - currentVelocity,
            ForceMode.VelocityChange
        );
    }
}
