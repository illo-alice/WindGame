using UnityEngine;

[DisallowMultipleComponent]
public sealed class GrappleModuleSource : MonoBehaviour, IMotorModuleSource
{
    [SerializeField] private ContentId _moduleId;
    [SerializeField] private AirflowModeController _airflowModeController;
    [SerializeField] private GrappleController _grappleController;

    [Header("Rope Physics")]
    [SerializeField, Min(0f)] private float _swingAcceleration = 25f;
    [SerializeField, Min(0f)] private float _stiffness = 40f;
    [SerializeField, Min(0f)] private float _damping = 8f;
    [SerializeField, Min(0f)] private float _maximumAcceleration = 80f;

    private GrappleMotorModule _module;

    public ContentId ModuleId => _moduleId;

    private void Awake()
    {
        _module = new GrappleMotorModule(
            _grappleController,
            _swingAcceleration,
            _stiffness,
            _damping,
            _maximumAcceleration
        );
    }

    public bool TryGetModule(
        in MotorContext context,
        out IMotorModule module)
    {
        if (_airflowModeController.Mode != AirflowMode.Suction)
        {
            _grappleController.Detach();
            module = null;
            return false;
        }

        var input = context.Input;
        _grappleController.Simulate(in input);

        if (!_grappleController.IsAttached)
        {
            module = null;
            return false;
        }

        module = _module;
        return true;
    }
}
