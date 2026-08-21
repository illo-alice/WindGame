using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class CheckPoint : NetworkBehaviour
{
    public int id;

    [SerializeField] private Transform _respawnPoint;
    
    private const byte AllPlayersMask = 0b00000011;

    [Networked]
    public byte ReachedPlayers { get; private set; }

    [Networked]
    public NetworkBool IsActivated { get; private set; }

    private CheckpointRegistry _checkpointRegistry;

    private void Awake()
    {
        _checkpointRegistry = GetComponentInParent<CheckpointRegistry>();
        _checkpointRegistry.Register(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority || IsActivated)
            return;

        var playerSlot =
            other.GetComponentInParent<PlayerSlot>();

        if (playerSlot == null)
            return;

        ReachedPlayers |=
            (byte)(1 << playerSlot.Index);

        if ((ReachedPlayers & AllPlayersMask) !=
            AllPlayersMask)
        {
            return;
        }

        Activate();
    }

    private void Activate()
    {
        IsActivated = true;
        _checkpointRegistry.CurrentCheckPointId = id;
        Debug.Log("Both players reached the checkpoint.");
    }

    public void StartFrom(Transform player)
    {
        if (HasStateAuthority)
        {
            player.position = _respawnPoint.position;
        }
    }
}
