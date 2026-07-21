using System;
using UnityEngine;

public sealed class ColliderScanner
{
    private Collider[] _results;

    public ColliderScanner(int capacity = 32)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        _results = new Collider[capacity];
    }

    public int Capacity => _results.Length;

    public int Scan(
        Vector3 origin,
        float radius,
        LayerMask colliderMask,
        QueryTriggerInteraction triggerInteraction =
            QueryTriggerInteraction.Ignore
    )
    {
        return Physics.OverlapSphereNonAlloc(
            origin,
            radius,
            _results,
            colliderMask,
            triggerInteraction
        );
    }

    public Collider GetResult(int index)
    {
        if ((uint)index >= (uint)_results.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        return _results[index];
    }

    public void SetCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        if (capacity == _results.Length)
            return;

        _results = new Collider[capacity];
    }
}