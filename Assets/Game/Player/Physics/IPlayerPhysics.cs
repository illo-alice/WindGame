using UnityEngine;

public interface IPlayerPhysics : IReadOnlyPlayerPhysics
{
    new Vector3 LinearVelocity { get; set; }
    Vector3 Position { get; set; }
}
