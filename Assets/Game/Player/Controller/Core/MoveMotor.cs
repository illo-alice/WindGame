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

    [Header("Modules")]
    [SerializeField] private MotorModuleInstaller _moduleInstaller;

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
        if (!GetInput(out InputData input))
            return;

        SynchronizeModuleCache();
        _groundSensor.Scan();

        var context = new MotorContext(
            input,
            PreviousButtons,
            _playerPhysics,
            _groundSensor,
            Runner.DeltaTime
        );

        _accumulator.Clear();

        foreach (var module in _cachedModules)
        {
            var forceData = module.Evaluate(in context);
            _accumulator.Add(in forceData);
        }

        _accumulator.Apply(_playerPhysics);
        PreviousButtons = input.buttons;
    }

    public bool TryAddModule(ContentId moduleConfigId)
    {
        if (!HasStateAuthority || !moduleConfigId.IsValid || !_cms.TryGet<MotorModuleDefinition>(moduleConfigId, out _))
        {
            return false;
        }

        if (ModuleIds.Count >= ModuleIds.Capacity)
            return false;

        ModuleIds.Add(moduleConfigId);
        return true;
    }

    public bool TryRemoveModule(ContentId moduleConfigId)
    {
        if (!HasStateAuthority || !moduleConfigId.IsValid)
            return false;

        return ModuleIds.Remove(moduleConfigId);
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
            _cachedModules.Add(_moduleFactory.Create(moduleId));
        }
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

            index++;
        }

        return true;
    }
}
