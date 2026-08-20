public interface IMotorConstraint
{
    void ApplyConstraint(
        in MotorContext context,
        IPlayerPhysics physics
    );
}
