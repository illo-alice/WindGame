using System.Collections.Generic;
using Fusion;
using UnityEngine;

public sealed class MoveMotor : NetworkBehaviour
{
    [Header("Physics")]
    [SerializeField] private PlayerPhysics _playerPhysics;
    [SerializeField] private GroundSensor _groundSensor;

    private readonly ForceAccumulator _accumulator = new();
    private readonly List<IMotorModule> _modules = new();
    private NetworkButtons _buttons;

    public void ClearModules()
    {
        _modules.Clear();
    }

    public void AddRangeOfModules(params IMotorModule[] modules)
    {
        _modules.AddRange(modules);
    }

    public void AddModule(IMotorModule module)
    {
        _modules.Add(module);
    }

    public void RemoveModule(IMotorModule module)
    {
        _modules.Remove(module);
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out InputData input))
            return;

        _groundSensor.Scan();

        var context = new MotorContext(
            input,
            _buttons,
            _playerPhysics,
            _groundSensor,
            Runner.DeltaTime
        );

        _accumulator.Clear();

        foreach (IMotorModule module in _modules)
        {
            ForceData forceData = module.Evaluate(in context);
            _accumulator.Add(in forceData);
        }

        _accumulator.Apply(_playerPhysics);
        _buttons = input.buttons;
    }
}
