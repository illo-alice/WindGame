using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(InputSlot))]
public sealed class PlayerColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField, FormerlySerializedAs("_host")] private Material _slot0;
    [SerializeField, FormerlySerializedAs("_client")] private Material _slot1;

    private InputSlot _inputSlot;

    public override void Spawned()
    {
        _inputSlot = GetComponent<InputSlot>();
        ApplyColor();
    }

    private void ApplyColor()
    {
        _meshRenderer.sharedMaterial =
            _inputSlot.Index == 0
                ? _slot0
                : _slot1;
    }
}
