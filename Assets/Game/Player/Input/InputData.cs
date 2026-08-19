using Fusion;
using UnityEngine;

public enum InputType
{
    Jump = 0,
    Sprint = 1,
    Fire = 2,
}

public struct PlayerInputData : INetworkStruct
{
    public NetworkButtons buttons;
    public Vector2 move;
    public Vector3 aimTarget;
}

public struct InputData : INetworkInput
{
    public PlayerInputData player0;
    public PlayerInputData player1;

    public PlayerInputData GetPlayer(byte index)
    {
        return index == 0
            ? player0
            : player1;
    }
}
