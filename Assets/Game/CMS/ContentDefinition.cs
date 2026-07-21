using System;
using Fusion;
using UnityEngine;

[Serializable]
public struct ContentId : INetworkStruct, IEquatable<ContentId>
{
    [SerializeField] private uint _value;

    public ContentId(uint value)
    {
        _value = value;
    }

    public uint Value => _value;
    public bool IsValid => _value != 0;

    public bool Equals(ContentId other) => _value == other._value;

    public override bool Equals(object obj) => obj is ContentId other && Equals(other);

    public override int GetHashCode() => unchecked((int)_value);

    public override string ToString() => _value.ToString();

    public static bool operator ==(ContentId left, ContentId right) => left.Equals(right);

    public static bool operator !=(ContentId left, ContentId right) => !left.Equals(right);
}

public abstract class ContentDefinition : ScriptableObject
{
    public abstract ContentId Id { get; }
}
