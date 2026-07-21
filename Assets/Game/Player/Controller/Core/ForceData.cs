using UnityEngine;

public readonly struct ForceData
{
    public ForceData(Vector3 force, ForceMode forceMode)
    {
        Force = force;
        ForceMode = forceMode;
    }

    public Vector3 Force { get; }
    public ForceMode ForceMode { get; }

    public static ForceData None => new(Vector3.zero, ForceMode.Force);
}
