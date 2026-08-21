using System.Collections.Generic;
using Fusion;

public class CheckpointRegistry : NetworkBehaviour
{
    [Networked]
    public int CurrentCheckPointId { get; set; }

    public CheckPoint CurrentCheckPoint => CheckPoints[CurrentCheckPointId];

    public Dictionary<int, CheckPoint> CheckPoints { get; } = new();
    
    public void Register(CheckPoint checkPoint)
    {
        CheckPoints[checkPoint.id] = checkPoint;
    }
}
