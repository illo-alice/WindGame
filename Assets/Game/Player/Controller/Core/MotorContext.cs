using Fusion;

public readonly struct MotorContext
{
    public MotorContext(
        InputData input,
        NetworkButtons previousButtons,
        IReadOnlyPlayerPhysics physics,
        IReadOnlyGroundSensor ground,
        float deltaTime
    )
    {
        Input = input;
        PreviousButtons = previousButtons;
        Physics = physics;
        Ground = ground;
        DeltaTime = deltaTime;
    }

    public InputData Input { get; }
    public NetworkButtons PreviousButtons { get; }
    public IReadOnlyPlayerPhysics Physics { get; }
    public IReadOnlyGroundSensor Ground { get; }
    public float DeltaTime { get; }
}
