using UnityEngine;

public sealed class PlayerPhysics : MonoBehaviour, IReadOnlyPlayerPhysics
{
    [SerializeField] private Rigidbody _rigidbody;
    
    public void AddForce(Vector3 force, ForceMode mode)
    {
        _rigidbody.AddForce(force, mode);
    }

    public Vector3 LinearVelocity
    {
        get => _rigidbody.linearVelocity;

        set => _rigidbody.linearVelocity = value;
    }

    public Vector3 HorizontalVelocity => new(LinearVelocity.x, 0f, LinearVelocity.z);
}
