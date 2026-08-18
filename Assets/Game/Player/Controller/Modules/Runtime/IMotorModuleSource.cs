public interface IMotorModuleSource
{
    ContentId ModuleId { get; }

    bool TryGetModule(
        in MotorContext context,
        out IMotorModule module
    );
}
