using Fusion;
using UnityEngine;

public readonly struct MotorContext
{
    public MotorContext(
        InputData input,
        NetworkButtons previousButtons,
        Transform playerTransform,
        IReadOnlyPlayerPhysics physics,
        IReadOnlyGroundSensor ground,
        float deltaTime
    )
    {
        Input = input;
        PreviousButtons = previousButtons;
        PlayerTransform = playerTransform;
        Physics = physics;
        Ground = ground;
        DeltaTime = deltaTime;
    }

    public InputData Input { get; }
    public NetworkButtons PreviousButtons { get; }
    public Transform PlayerTransform { get; }
    public IReadOnlyPlayerPhysics Physics { get; }
    public IReadOnlyGroundSensor Ground { get; }
    public float DeltaTime { get; }
}
