using System.Collections.Generic;

public abstract class MotorModuleDefinition : ContentDefinition
{
    public abstract IMotorModule CreateModule();
}

public sealed class MotorModuleFactory
{
    private readonly ICMS _cms;

    public MotorModuleFactory(ICMS cms)
    {
        _cms = cms;
    }

    public IMotorModule Create(in ContentId id)
    {
        if (!_cms.TryGet(id, out MotorModuleDefinition definition))
        {
            throw new KeyNotFoundException(
                $"Motor module config '{id}' was not found in CMS."
            );
        }

        return definition.CreateModule();
    }
}
