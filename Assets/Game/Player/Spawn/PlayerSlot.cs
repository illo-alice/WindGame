using Fusion;
using UnityEngine;

public sealed class PlayerSlot : NetworkBehaviour
{
    [Networked]
    public byte Index { get; private set; }

    public void SetIndex(byte index)
    {
        if (!HasStateAuthority)
            return;

        Index = index;
    }
}