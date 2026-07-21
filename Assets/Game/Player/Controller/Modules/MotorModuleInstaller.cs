using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MoveMotor))]
public sealed class MotorModuleInstaller : MonoBehaviour
{
    [SerializeField] private MotorModuleDefinition[] _initialModules =
        Array.Empty<MotorModuleDefinition>();

    public IReadOnlyList<ContentId> InitialModules 
        => _initialModules.Select(motorModuleDefinition => motorModuleDefinition.Id).ToList();
}
