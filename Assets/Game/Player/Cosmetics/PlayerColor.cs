using Fusion;
using UnityEngine;

public sealed class PlayerColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _host;
    [SerializeField] private Material _client;

    [Networked, OnChangedRender(nameof(ApplyColor))]
    private NetworkBool IsHostPlayer { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            IsHostPlayer =
                Object.InputAuthority == Runner.LocalPlayer;
        }

        ApplyColor();
    }

    private void ApplyColor()
    {
        _meshRenderer.sharedMaterial =
            IsHostPlayer ? _host : _client;
    }
}