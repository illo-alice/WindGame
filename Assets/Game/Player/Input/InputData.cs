using Fusion;
using UnityEngine;

public enum InputType
{
    Jump = 0,
    Sprint = 1,
    Fire = 2,
}

public struct InputData : INetworkInput
{
    public NetworkButtons buttons;
    public Vector2 move;
    public Vector3 aimTarget;
}