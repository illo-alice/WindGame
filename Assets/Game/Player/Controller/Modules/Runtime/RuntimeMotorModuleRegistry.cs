using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class RuntimeMotorModuleRegistry : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] _sourceBehaviours =
        Array.Empty<MonoBehaviour>();

    private readonly List<IMotorModuleSource> _sources = new();
    private readonly Dictionary<ContentId, IMotorModuleSource> _sourcesById = new();
    private readonly Dictionary<ContentId, IMotorModule> _modulesById = new();

    private void Awake()
    {
        _sources.Clear();
        _sourcesById.Clear();
        _modulesById.Clear();

        foreach (var behaviour in _sourceBehaviours)
        {
            if (behaviour is not IMotorModuleSource source)
            {
                throw new InvalidOperationException(
                    $"'{behaviour?.name ?? "Null"}' on '{name}' does not implement " +
                    $"{nameof(IMotorModuleSource)}."
                );
            }

            if (!source.ModuleId.IsValid)
            {
                throw new InvalidOperationException(
                    $"{source.GetType().Name} on '{name}' has an invalid module ID."
                );
            }

            if (!_sourcesById.TryAdd(source.ModuleId, source))
            {
                throw new InvalidOperationException(
                    $"Runtime motor module ID '{source.ModuleId}' is registered more than once " +
                    $"on '{name}'."
                );
            }

            _sources.Add(source);
        }
    }

    public void Synchronize(in MotorContext context, MoveMotor motor)
    {
        foreach (var source in _sources)
        {
            var isActive = source.TryGetModule(in context, out var module);

            if (isActive)
            {
                if (module == null)
                {
                    throw new InvalidOperationException(
                        $"{source.GetType().Name} returned true with a null module."
                    );
                }

                _modulesById[source.ModuleId] = module;
            }

            motor.SetModuleActive(source.ModuleId, isActive);
        }
    }

    public bool Contains(ContentId moduleId) =>
        _sourcesById.ContainsKey(moduleId);

    public bool TryResolve(ContentId moduleId, out IMotorModule module) =>
        _modulesById.TryGetValue(moduleId, out module);
}
