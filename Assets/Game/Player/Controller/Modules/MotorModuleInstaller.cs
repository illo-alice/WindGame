using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MoveMotor))]
public sealed class MotorModuleInstaller : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _acceleration = 30f;
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _sprintMaxSpeed = 12f;
    [SerializeField] private float _sprintAccelerationMultiplier = 1.5f;
    [SerializeField] private float _groundDeceleration = 40f;
    [SerializeField] private float _airDeceleration = 5f;

    [Header("Jump and Gravity")]
    [SerializeField] private float _jumpForce = 14f;
    [SerializeField] private float _upGravity = 25f;
    [SerializeField] private float _downGravity = 40f;

    [Header("Reference")]
    [SerializeField] private MoveMotor _motor;

    private void Awake()
    {
        _motor.ClearModules();
        _motor.AddRangeOfModules(
            new JumpModule(_jumpForce),
            new GravityModule(_upGravity, _downGravity),
            new HorizontalModule(
                _acceleration,
                _maxSpeed,
                _sprintMaxSpeed,
                _sprintAccelerationMultiplier
            ),
            new GroundDecelerationModule(_groundDeceleration),
            new AirDecelerationModule(_airDeceleration)
        );
    }

    private void OnValidate()
    {
        if (_motor == null)
            _motor = GetComponent<MoveMotor>();
    }
}
