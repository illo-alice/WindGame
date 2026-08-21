using System;
using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InputSlot : NetworkBehaviour
{
    [Networked] public byte Index { get; private set; }

    public void SetIndex(byte index)
    {
        if (index > 1)
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                "Only local player slots 0 and 1 are supported."
            );

        if (!HasStateAuthority)
            throw new InvalidOperationException(
                "Only the state authority can assign a local player slot."
            );

        Index = index;
    }
}
