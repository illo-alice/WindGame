using UnityEngine;

[CreateAssetMenu(fileName = "Gravity Module", menuName = "Content/Motor Modules/Gravity")]
public sealed class GravityModuleConfig : MotorModuleDefinition
{
    [SerializeField] private float _upGravity = 25f;
    [SerializeField] private float _downGravity = 40f;
    [SerializeField] private ContentId _contentId;
    public override ContentId Id => _contentId;

    public override IMotorModule CreateModule()
    {
        return new GravityModule(_upGravity, _downGravity);
    }
}
