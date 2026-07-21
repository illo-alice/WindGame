using UnityEngine;

public sealed class GravityModule : IMotorModule
{
    private readonly float _upGravity;
    private readonly float _downGravity;

    public GravityModule(float upGravity, float downGravity)
    {
        _upGravity = upGravity;
        _downGravity = downGravity;
    }

    public ForceData Evaluate(in MotorContext context)
    {
        if (context.Ground.IsGrounded && context.Physics.LinearVelocity.y <= 0f)
            return ForceData.None;

        float targetGravity = context.Physics.LinearVelocity.y >= 0f
            ? _upGravity
            : _downGravity;

        float defaultGravity = Mathf.Abs(Physics.gravity.y);
        float additionalGravity = Mathf.Max(0f, targetGravity - defaultGravity);

        return new ForceData(
            Vector3.down * additionalGravity,
            ForceMode.Acceleration
        );
    }
}
