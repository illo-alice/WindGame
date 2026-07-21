using UnityEngine;

[CreateAssetMenu(fileName = "Air Deceleration Module", menuName = "Content/Motor Modules/Air Deceleration")]
public sealed class AirDecelerationModuleConfig : MotorModuleDefinition
{
    [SerializeField] private float _deceleration = 5f;
    [SerializeField] private ContentId _contentId;
    public override ContentId Id => _contentId;
    public override IMotorModule CreateModule()
    {
        return new AirDecelerationModule(_deceleration);
    }
}
