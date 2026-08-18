using Fusion;
using UnityEngine;

public enum AirflowMode : byte
{
    Blow = 0,
    Suction = 1,
}

[DisallowMultipleComponent]
public sealed class AirflowModeController : NetworkBehaviour
{
    [SerializeField] private AirflowMode _initialMode = AirflowMode.Suction;

    [Networked] public AirflowMode Mode { get; private set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
            Mode = _initialMode;
    }

    public bool TrySetMode(AirflowMode mode)
    {
        if (!HasStateAuthority)
            return false;

        Mode = mode;
        return true;
    }
}
