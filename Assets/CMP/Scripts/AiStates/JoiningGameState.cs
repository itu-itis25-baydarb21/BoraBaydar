using System.Collections;
using System.Collections.Generic;
using CMP.Scripts;
using UnityEngine;

namespace CMP.Scripts.AiStates 
{
    public class JoiningGameState : GhostState
    {
        private Vector3 _gateWorldPos;
        private Vector3 _targetWorldPos;
        private bool _alignedWithGateX = false;
        public JoiningGameState(GhostBlackboard blackboard) : base(blackboard) {}

        public override void OnEnter()
        {
            var joinGameCell = GhostBlackboard.GridData.GetCoordsOfCellType(CellType.JoinGameCell);

            if(joinGameCell != null && joinGameCell.Count > 0)
            {
                _gateWorldPos = (Vector3)(Vector2)joinGameCell[0];

                _targetWorldPos = new Vector3(_gateWorldPos.x,GhostBlackboard.GhostTransform.transform.position.y, GhostBlackboard.GhostTransform.position.z);
            }
            else
            {
                Debug.LogError("JoinGameCell cannot found");
            }   
        }

        public override void Update()
        {
            GhostBlackboard.GhostTransform.position = Vector3.MoveTowards(
                GhostBlackboard.GhostTransform.position,
                _targetWorldPos,
                GhostBlackboard.Speed * Time.deltaTime
            );

            if (Vector3.Distance(GhostBlackboard.GhostTransform.position, _targetWorldPos) < 0.01f)
            {
                if (!_alignedWithGateX)
                {
                    _alignedWithGateX = true;
                    _targetWorldPos = _gateWorldPos;
                }
                else
                {
                    GhostBlackboard.CurrentGridPos = new Vector2Int(
                        Mathf.RoundToInt(_gateWorldPos.x), 
                        Mathf.RoundToInt(_gateWorldPos.y)
                    );
                    GhostBlackboard.GhostComponent.ChangeState(new ScatterState(GhostBlackboard));
                }
            }       
        }
    }
}