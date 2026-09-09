using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ChaseState : GhostState
    {
        private Vector2Int _currentTile;
        private Vector2Int _targetTile;
        private Vector2Int _currentDirection;

        private const float CatchDistance = 0.4f; 
        private const float ArrivedTolerance = 0.02f;

        private static readonly Vector2Int[] PossibleDirections = new[]
        {
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down,
            Vector2Int.right
        };

        public ChaseState(GhostBlackboard blackboard) : base(blackboard) { }

        public override void OnEnter()
        {
            _currentTile = GhostBlackboard.CurrentGridPos;
            
            _currentDirection = GhostBlackboard.GridDirection != Vector2Int.zero 
                ? GhostBlackboard.GridDirection 
                : Vector2Int.up;

            _targetTile = _currentTile + _currentDirection;
            GhostBlackboard.TargetGridPos = _targetTile;
        }

        public override void Update()
        {
            if (GhostBlackboard.PacmanTransform != null)
            {
                float distanceToPacman = Vector3.Distance(
                    GhostBlackboard.GhostTransform.position,
                    GhostBlackboard.PacmanTransform.position
                );

                if (distanceToPacman <= CatchDistance)
                {
                    GhostBlackboard.GameManager?.TriggerGameOver();
                    return;
                }
            }

            Vector3 targetWorldPos = new Vector3(_targetTile.x, _targetTile.y, GhostBlackboard.GhostTransform.position.z);
            GhostBlackboard.GhostTransform.position = Vector3.MoveTowards(
                GhostBlackboard.GhostTransform.position,
                targetWorldPos,
                GhostBlackboard.Speed * Time.deltaTime
            );

            if (Vector3.Distance(GhostBlackboard.GhostTransform.position, targetWorldPos) <= ArrivedTolerance)
            {
                GhostBlackboard.GhostTransform.position = targetWorldPos;
                _currentTile = _targetTile;
                GhostBlackboard.CurrentGridPos = _currentTile;

                _currentDirection = ChooseBestDirectionToPacman(_currentTile, _currentDirection);
                GhostBlackboard.GridDirection = _currentDirection;

                _targetTile = _currentTile + _currentDirection;
                GhostBlackboard.TargetGridPos = _targetTile;
            }
        }

        private Vector2Int ChooseBestDirectionToPacman(Vector2Int atTile, Vector2Int comingDirection)
        {
            if (GhostBlackboard.PacmanTransform == null)
                return comingDirection;

            Vector2Int pacmanTile = new Vector2Int(
                Mathf.RoundToInt(GhostBlackboard.PacmanTransform.position.x),
                Mathf.RoundToInt(GhostBlackboard.PacmanTransform.position.y)
            );

            Vector2Int oppositeDirection = -comingDirection;
            List<Vector2Int> validDirections = new List<Vector2Int>();

            foreach (var dir in PossibleDirections)
            {
                Vector2Int neighbor = atTile + dir;
                if (IsTileWalkable(neighbor))
                {
                    validDirections.Add(dir);
                }
            }

            if (validDirections.Count > 1 && comingDirection != Vector2Int.zero)
            {
                validDirections.Remove(oppositeDirection);
            }

            if (validDirections.Count == 0)
                return Vector2Int.zero;

            Vector2Int bestDirection = validDirections[0];
            float shortestDistance = float.MaxValue;

            foreach (var dir in validDirections)
            {
                Vector2Int neighbor = atTile + dir;
                float sqrDist = (neighbor - pacmanTile).sqrMagnitude;

                if (sqrDist < shortestDistance)
                {
                    shortestDistance = sqrDist;
                    bestDirection = dir;
                }
            }

            return bestDirection;
        }

        private bool IsTileWalkable(Vector2Int coord)
        {
            if (coord.x < 0 || coord.x >= GhostBlackboard.GridData.Width ||
                coord.y < 0 || coord.y >= GhostBlackboard.GridData.Height)
            {
                return false;
            }

            if (GhostBlackboard.NotMovableCellCoords != null && 
                GhostBlackboard.NotMovableCellCoords.Contains(coord))
            {
                return false;
            }

            CellType cellType = GhostBlackboard.GridData.GetCellAt(coord);
            if (cellType == CellType.Wall || 
                cellType == CellType.AiSpawnZone || 
                cellType == CellType.AiGate)
            {
                return false;
            }

            return true;
        }
    }
}