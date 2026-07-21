using Fusion;
using UnityEngine;

public sealed class JumpModule : IMotorModule
{
    private readonly float _jumpForce;

    public JumpModule(float jumpForce)
    {
        _jumpForce = jumpForce;
    }

    public ForceData Evaluate(in MotorContext context)
    {
        bool jumpPressed = context.Input.buttons.WasPressed(
            context.PreviousButtons,
            InputType.Jump
        );

        if (!jumpPressed || !context.Ground.IsGrounded)
            return ForceData.None;

        return new ForceData(
            Vector3.up * _jumpForce,
            ForceMode.VelocityChange
        );
    }
}
