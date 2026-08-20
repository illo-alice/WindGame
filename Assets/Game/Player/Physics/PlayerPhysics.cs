using UnityEngine;

public sealed class PlayerPhysics : MonoBehaviour, IPlayerPhysics
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

    public Vector3 Position
    {
        get => _rigidbody.position;
        set => _rigidbody.position = value;
    }

    public Vector3 HorizontalVelocity => new(LinearVelocity.x, 0f, 0f);
}
