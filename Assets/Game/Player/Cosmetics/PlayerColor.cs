using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LocalPlayerSlot))]
public sealed class PlayerColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField, FormerlySerializedAs("_host")] private Material _slot0;
    [SerializeField, FormerlySerializedAs("_client")] private Material _slot1;

    private LocalPlayerSlot _localPlayerSlot;

    public override void Spawned()
    {
        _localPlayerSlot = GetComponent<LocalPlayerSlot>();
        ApplyColor();
    }

    private void ApplyColor()
    {
        _meshRenderer.sharedMaterial =
            _localPlayerSlot.Index == 0
                ? _slot0
                : _slot1;
    }
}
