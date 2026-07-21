using UnityEngine;

public interface IReadOnlyPlayerPhysics
{
    Vector3 LinearVelocity { get; }
    Vector3 HorizontalVelocity { get; }
}
