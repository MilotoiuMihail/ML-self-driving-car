using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : MonoBehaviour
{
    [field: SerializeField] public Track Track { get; private set; }
    [field: SerializeField] public Grid Grid { get; private set; }
    [field: SerializeField] private Transform obstaclesParent;
    public List<TrackPiece> TrackPieces { get; private set; } = new List<TrackPiece>();
    public List<Obstacle> Obstacles { get; private set; } = new List<Obstacle>();

    public void UpdateEnvironment()
    {
        TrackPieces.Clear();
        TrackPieces.AddRange(Track.GetComponentsInChildren<TrackPiece>().Where(piece => piece.IsPlaced));
        Obstacles.Clear();
        Obstacles.AddRange(obstaclesParent.GetComponentsInChildren<Obstacle>().Where(obstacle => obstacle.IsPlaced));
    }
    public void ResetObstacles()
    {
        foreach (Obstacle obstacle in Obstacles)
        {
            Destroy(obstacle.gameObject);
        }
        Obstacles.Clear();
    }
}
