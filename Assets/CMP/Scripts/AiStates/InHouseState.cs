using System;
using System.Collections;
using System.Collections.Generic;
using CMP.Scripts;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
    public class InHouseState : GhostState
    {
        private Vector2Int _homeCenter;
        private Vector2Int _targetTile;
        private bool _isMovingUp = true;
        public InHouseState(GhostBlackboard blackboard) : base(blackboard)
        {
        }

        public override void OnEnter()
        {
            GhostBlackboard.StateTimer = 0f;

            _homeCenter = GhostBlackboard.CurrentGridPos;

            _targetTile = _homeCenter + Vector2Int.up;
            _isMovingUp = true;
        }

        public override void Update()
        {
            GhostBlackboard.StateTimer += Time.deltaTime;

            Vector3 targetWorldPos = new Vector3(_targetTile.x, _targetTile.y, GhostBlackboard.GhostTransform.position.z);
            GhostBlackboard.GhostTransform.position = Vector3.MoveTowards(
                GhostBlackboard.GhostTransform.position,
                targetWorldPos,
                GhostBlackboard.Speed * Time.deltaTime
            );

            if(Vector3.Distance(GhostBlackboard.GhostTransform.position, targetWorldPos) < GameSettings.TileArrivedTolerance)
            {
                if(_isMovingUp)
                {
                    _targetTile = _homeCenter;
                    _isMovingUp = false;
                }
                else
                {
                    _targetTile = _homeCenter + Vector2Int.up;
                    _isMovingUp = true;
                }
            }

            if(GhostBlackboard.StateTimer >= GhostBlackboard.JoinDelay)
            {
                GhostBlackboard.GhostComponent.ChangeState(new JoiningGameState(GhostBlackboard));
            }
        }
    }
}
