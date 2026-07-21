using System.Collections.Generic;

public interface ICMS
{
    bool TryGet<TDefinition>(ContentId id, out TDefinition definition)
        where TDefinition : ContentDefinition;

    TDefinition Get<TDefinition>(ContentId id)
        where TDefinition : ContentDefinition;

    IEnumerable<TDefinition> GetAll<TDefinition>()
        where TDefinition : ContentDefinition;
}
