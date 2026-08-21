using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField] private CheckpointRegistry _checkpointRegistry;
    
    private void OnTriggerEnter(Collider other)
    {
        var playerTransform = other.GetComponentInParent<Transform>();
        
        _checkpointRegistry.CurrentCheckPoint.StartFrom(playerTransform);
    }
}
