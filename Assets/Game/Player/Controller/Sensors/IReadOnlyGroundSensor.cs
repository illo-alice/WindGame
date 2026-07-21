using UnityEngine;

public interface IReadOnlyGroundSensor
{
    bool IsGrounded { get; }
    Vector3 GroundNormal { get; }
    Collider GroundCollider { get; }
}
