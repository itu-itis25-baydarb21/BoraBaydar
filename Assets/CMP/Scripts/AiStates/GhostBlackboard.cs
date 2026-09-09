using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class GhostBlackboard
    {
        public Transform GhostTransform;
        public Ghost GhostComponent;

        public Transform PacmanTransform;
        public GridData GridData;
        public HashSet<Vector2Int> NotMovableCellCoords;
        public GameManager GameManager;

        public Vector2Int CurrentGridPos;
        public Vector2Int TargetGridPos;
        public Direction CurrentDirection = Direction.None;
        public Vector2Int GridDirection = Vector2Int.zero; 
        
        public float Speed = 3f;
        public float StateTimer;
        public float JoinDelay;
    }
}