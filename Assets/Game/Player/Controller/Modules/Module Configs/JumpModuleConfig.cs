using UnityEngine;

[CreateAssetMenu(fileName = "Jump Module", menuName = "Content/Motor Modules/Jump")]
public sealed class JumpModuleConfig : MotorModuleDefinition
{
    [SerializeField] private float _jumpForce = 14f;
    [SerializeField] private ContentId _contentId;
    public override ContentId Id => _contentId;

    public override IMotorModule CreateModule()
    {
        return new JumpModule(_jumpForce);
    }
}
