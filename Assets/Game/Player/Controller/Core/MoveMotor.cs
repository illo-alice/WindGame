using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using VContainer;

public sealed class MoveMotor : NetworkBehaviour
{
    private const int ModuleCapacity = 16;

    [Header("Physics")]
    [SerializeField] private PlayerPhysics _playerPhysics;
    [SerializeField] private GroundSensor _groundSensor;
    [SerializeField] private LocalPlayerSlot _localPlayerSlot;

    [Header("Modules")]
    [SerializeField] private MotorModuleInstaller _moduleInstaller;
    [SerializeField] private RuntimeMotorModuleRegistry _runtimeModuleRegistry;

    [Networked, Capacity(ModuleCapacity)]
    private NetworkLinkedList<ContentId> ModuleIds => default;

    [Networked] private NetworkButtons PreviousButtons { get; set; }

    private readonly ForceAccumulator _accumulator = new();
    private readonly List<ContentId> _cachedModuleIds = new(ModuleCapacity);
    private readonly List<IMotorModule> _cachedModules = new(ModuleCapacity);

    private MotorModuleFactory _moduleFactory;
    private ICMS _cms;

    [Inject]
    public void Construct(MotorModuleFactory moduleFactory, ICMS cms)
    {
        _moduleFactory = moduleFactory;
        _cms = cms;
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
            SetModules(_moduleInstaller.InitialModules);
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out InputData combinedInput))
            return;

        var input = combinedInput.GetPlayer(_localPlayerSlot.Index);

        _groundSensor.Scan();

        var context = new MotorContext(
            input,
            PreviousButtons,
            transform,
            _playerPhysics,
            _groundSensor,
            Runner.DeltaTime
        );

        _runtimeModuleRegistry.Synchronize(in context, this);
        SynchronizeModuleCache();

        _accumulator.Clear();

        foreach (var module in _cachedModules)
        {
            var forceData = module.Evaluate(in context);
            _accumulator.Add(in forceData);
        }

        _accumulator.Apply(_playerPhysics);
        ApplyConstraints(in context);
        PreviousButtons = input.buttons;
    }

    private void ApplyConstraints(in MotorContext context)
    {
        foreach (var module in _cachedModules)
        {
            if (module is IMotorConstraint constraint)
                constraint.ApplyConstraint(in context, _playerPhysics);
        }
    }

    public bool TryAddModule(ContentId moduleConfigId)
    {
        if (!CanChangeModules || !IsKnownModule(moduleConfigId))
            return false;

        return SetModuleActive(moduleConfigId, true);
    }

    public bool TryRemoveModule(ContentId moduleConfigId)
    {
        if (!CanChangeModules || !moduleConfigId.IsValid)
            return false;

        return SetModuleActive(moduleConfigId, false);
    }

    public bool SetModuleActive(ContentId moduleId, bool isActive)
    {
        if (!CanChangeModules || !IsKnownModule(moduleId))
            return false;

        var contains = ContainsModule(moduleId);

        if (contains == isActive)
            return true;

        if (!isActive)
            return ModuleIds.Remove(moduleId);

        if (ModuleIds.Count >= ModuleIds.Capacity)
            return false;

        ModuleIds.Add(moduleId);
        return true;
    }

    private void SetModules(IReadOnlyList<ContentId> moduleConfigIds)
    {
        ModuleIds.Clear();

        foreach (var moduleConfigId in moduleConfigIds)
            ModuleIds.Add(moduleConfigId);
    }

    private void SynchronizeModuleCache()
    {
        if (IsModuleCacheCurrent())
            return;

        _cachedModuleIds.Clear();
        _cachedModules.Clear();

        foreach (var moduleId in ModuleIds)
        {
            _cachedModuleIds.Add(moduleId);

            if (_runtimeModuleRegistry.TryResolve(moduleId, out var runtimeModule))
            {
                _cachedModules.Add(runtimeModule);
                continue;
            }

            if (_runtimeModuleRegistry.Contains(moduleId))
            {
                throw new InvalidOperationException(
                    $"Runtime motor module '{moduleId}' is active but was not resolved."
                );
            }

            _cachedModules.Add(_moduleFactory.Create(moduleId));
        }
    }

    private bool CanChangeModules =>
        HasStateAuthority || HasInputAuthority;

    private bool IsKnownModule(ContentId moduleId) =>
        moduleId.IsValid &&
        (_runtimeModuleRegistry.Contains(moduleId) ||
         _cms.TryGet<MotorModuleDefinition>(moduleId, out _));

    private bool ContainsModule(ContentId moduleId)
    {
        foreach (var activeModuleId in ModuleIds)
        {
            if (activeModuleId == moduleId)
                return true;
        }

        return false;
    }

    private bool IsModuleCacheCurrent()
    {
        if (_cachedModuleIds.Count != ModuleIds.Count)
            return false;

        var index = 0;

        foreach (var moduleId in ModuleIds)
        {
            if (_cachedModuleIds[index] != moduleId)
                return false;

            if (_runtimeModuleRegistry.TryResolve(moduleId, out var runtimeModule) &&
                !ReferenceEquals(_cachedModules[index], runtimeModule))
            {
                return false;
            }

            index++;
        }

        return true;
    }
}
