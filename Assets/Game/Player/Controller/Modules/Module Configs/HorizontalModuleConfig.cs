using UnityEngine;

[CreateAssetMenu(fileName = "Horizontal Module", menuName = "Content/Motor Modules/Horizontal")]
public sealed class HorizontalModuleConfig : MotorModuleDefinition
{
    [SerializeField] private float _acceleration = 30f;
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _sprintMaxSpeed = 12f;
    [SerializeField] private float _sprintAccelerationMultiplier = 1.5f;
    [SerializeField] private ContentId _contentId;
    public override ContentId Id => _contentId;

    public override IMotorModule CreateModule()
    {
        return new HorizontalModule(
            _acceleration,
            _maxSpeed,
            _sprintMaxSpeed,
            _sprintAccelerationMultiplier
        );
    }
}
