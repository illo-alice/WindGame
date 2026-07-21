using System;
using UnityEngine;

public sealed class ForceAccumulator
{
    private Vector3 _force;
    private Vector3 _acceleration;
    private Vector3 _impulse;
    private Vector3 _velocityChange;

    public void Clear()
    {
        _force = Vector3.zero;
        _acceleration = Vector3.zero;
        _impulse = Vector3.zero;
        _velocityChange = Vector3.zero;
    }

    public void Add(in ForceData forceData)
    {
        switch (forceData.ForceMode)
        {
            case ForceMode.Force:
                _force += forceData.Force;
                break;

            case ForceMode.Acceleration:
                _acceleration += forceData.Force;
                break;

            case ForceMode.Impulse:
                _impulse += forceData.Force;
                break;

            case ForceMode.VelocityChange:
                _velocityChange += forceData.Force;
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(forceData),
                    forceData.ForceMode,
                    "Unsupported force mode."
                );
        }
    }

    public void Apply(PlayerPhysics playerPhysics)
    {
        if (playerPhysics == null)
            throw new ArgumentNullException(nameof(playerPhysics));

        ApplyIfNotZero(playerPhysics, _force, ForceMode.Force);
        ApplyIfNotZero(playerPhysics, _acceleration, ForceMode.Acceleration);
        ApplyIfNotZero(playerPhysics, _impulse, ForceMode.Impulse);
        ApplyIfNotZero(playerPhysics, _velocityChange, ForceMode.VelocityChange);
    }

    private static void ApplyIfNotZero(
        PlayerPhysics playerPhysics,
        Vector3 force,
        ForceMode forceMode
    )
    {
        if (force.sqrMagnitude <= 0.000001f)
            return;

        playerPhysics.AddForce(force, forceMode);
    }
}
