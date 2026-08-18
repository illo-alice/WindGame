using UnityEngine;

[CreateAssetMenu(fileName = "Ground Deceleration Module", menuName = "Content/Motor Modules/Ground Deceleration")]
public sealed class GroundDecelerationModuleConfig : MotorModuleDefinition
{
    [SerializeField] private float _deceleration = 40f;
    [SerializeField] private ContentId _contentId;
    public override ContentId Id => _contentId;

    public override IMotorModule CreateModule()
    {
        return new GroundDecelerationModule(_deceleration);
    }
}
