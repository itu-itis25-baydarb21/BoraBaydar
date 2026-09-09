using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class ScatterState : GhostState
    {
        private Vector2Int _currentTile;
        private Vector2Int _targetTile;
        private Vector2Int _currentDirection = Vector2Int.zero;

        private const float SightDistance = 6f; 
        private const float ArrivedTolerance = 0.02f;

        private static readonly Vector2Int[] PossibleDirections = new[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        public ScatterState(GhostBlackboard blackboard) : base(blackboard) { }

        public override void OnEnter()
        {
            _currentTile = GhostBlackboard.CurrentGridPos;

            _currentDirection = ChooseNextDirection(_currentTile, GhostBlackboard.GridDirection);
            GhostBlackboard.GridDirection = _currentDirection;

            _targetTile = _currentTile + _currentDirection;
            GhostBlackboard.TargetGridPos = _targetTile;
        }

        public override void Update()
        {
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

                _currentDirection = ChooseNextDirection(_currentTile, _currentDirection);
                GhostBlackboard.GridDirection = _currentDirection;

                _targetTile = _currentTile + _currentDirection;
                GhostBlackboard.TargetGridPos = _targetTile;
            }

            if (HasLineOfSightToPacman(SightDistance))
            {
                GhostBlackboard.GameManager?.TriggerAllGhostsChase();
            }
        }

        private Vector2Int ChooseNextDirection(Vector2Int atTile, Vector2Int comingDirection)
        {
            List<Vector2Int> validDirections = new List<Vector2Int>();
            Vector2Int oppositeDirection = -comingDirection;

            foreach (var dir in PossibleDirections)
            {
                Vector2Int neighbor = atTile + dir;

                if (!IsTileWalkable(neighbor)) 
                    continue;

                validDirections.Add(dir);
            }

            if (validDirections.Count > 1 && comingDirection != Vector2Int.zero)
            {
                validDirections.Remove(oppositeDirection);
            }

            if (validDirections.Count == 0)
                return Vector2Int.zero;

            int randomIndex = Random.Range(0, validDirections.Count);
            return validDirections[randomIndex];
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

        private bool HasLineOfSightToPacman(float maxVisionDistance)
        {
            if (GhostBlackboard.PacmanTransform == null) return false;

            Vector2Int ghostTile = GhostBlackboard.CurrentGridPos;
            Vector2Int pacmanTile = new Vector2Int(
                Mathf.RoundToInt(GhostBlackboard.PacmanTransform.position.x),
                Mathf.RoundToInt(GhostBlackboard.PacmanTransform.position.y)
            );

            int distance = Mathf.Abs(ghostTile.x - pacmanTile.x) + Mathf.Abs(ghostTile.y - pacmanTile.y);
            if (distance > maxVisionDistance) return false;

            Vector2Int checkStep = Vector2Int.zero;

            if (_currentDirection == Vector2Int.right && ghostTile.y == pacmanTile.y && pacmanTile.x > ghostTile.x)
                checkStep = Vector2Int.right;
            else if (_currentDirection == Vector2Int.left && ghostTile.y == pacmanTile.y && pacmanTile.x < ghostTile.x)
                checkStep = Vector2Int.left;
            else if (_currentDirection == Vector2Int.up && ghostTile.x == pacmanTile.x && pacmanTile.y > ghostTile.y)
                checkStep = Vector2Int.up;
            else if (_currentDirection == Vector2Int.down && ghostTile.x == pacmanTile.x && pacmanTile.y < ghostTile.y)
                checkStep = Vector2Int.down;
            else
                return false;

            Vector2Int current = ghostTile + checkStep;
            while (current != pacmanTile)
            {
                if (GhostBlackboard.GridData.GetCellAt(current) == CellType.Wall)
                    return false;

                current += checkStep;
            }

            return true;
        }
    }
}