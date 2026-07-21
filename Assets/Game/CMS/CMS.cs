using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Content Catalog", menuName = "Content/Content Catalog")]
public sealed class CMS : ScriptableObject, ICMS
{
    [SerializeField] private List<ContentDefinition> _definitions = new();

    private readonly Dictionary<ContentId, ContentDefinition> _byId = new();

    public bool BuildIndex()
    {
        _byId.Clear();
        bool isValid = true;

        foreach (var definition in _definitions)
        {
            if (definition == null)
                continue;

            if (!definition.Id.IsValid)
            {
                Debug.LogError(
                    $"Content definition '{definition.name}' has an invalid ID.",
                    definition);
                isValid = false;
                continue;
            }

            if (!_byId.TryAdd(definition.Id, definition))
            {
                var existing = _byId[definition.Id];
                Debug.LogError(
                    $"Duplicate content ID {definition.Id}: " +
                    $"'{existing.name}' and '{definition.name}'.",
                    this);
                isValid = false;
            }
        }

        return isValid;
    }

    public bool TryGet<TDefinition>(ContentId id, out TDefinition definition)
        where TDefinition : ContentDefinition
    {
        if (_byId.TryGetValue(id, out var raw) &&
            raw is TDefinition typed)
        {
            definition = typed;
            return true;
        }

        definition = null;
        return false;
    }

    public TDefinition Get<TDefinition>(ContentId id)
        where TDefinition : ContentDefinition
    {
        if (TryGet(id, out TDefinition definition))
            return definition;

        throw new KeyNotFoundException(
            $"Content definition '{id}' of type {typeof(TDefinition).Name} was not found.");
    }

    public IEnumerable<TDefinition> GetAll<TDefinition>()
        where TDefinition : ContentDefinition
    {
        for (var i = 0; i < _definitions.Count; i++)
        {
            if (_definitions[i] is TDefinition typed)
                yield return typed;
        }
    }
}
