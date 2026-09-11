using System.Collections.Generic;
using CMP.Scripts.AiStates;
using UnityEngine;

using GhostStateBase = CMP.Scripts.AiStates.GhostState;

namespace CMP.Scripts
{
    public class Ghost : MonoBehaviour
    {
        public GameObject LeftEye;
        public GameObject RightEye;

        public GhostBlackboard Blackboard { get; private set; }
        public GhostStateBase CurrentState { get; private set; }
        public InHouseState InHouseState { get; private set; }

        public void Initialize(GridData gridData, Transform pacmanTransform, GameManager gameManager, float joinDelay)
        {
            Blackboard = new GhostBlackboard
            {
                GhostTransform = transform,
                GhostComponent = this,
                GridData = gridData,
                PacmanTransform = pacmanTransform,
                GameManager = gameManager,
                JoinDelay = joinDelay,
                Speed = 1f / GameSettings.AiMovementDuration,
                CurrentGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)),
                TargetGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y))
            };

            InHouseState = new InHouseState(Blackboard);
            ChangeState(InHouseState);
        }

        public void Update()
        {
            CurrentState?.Update();
            UpdateEyeDirection();
        }

        public void ChangeState(GhostStateBase newState)
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }

        private void UpdateEyeDirection()
        {
            if (Blackboard == null || (LeftEye == null && RightEye == null)) return;

            Vector2Int dir = Blackboard.GridDirection;
            if (dir == Vector2Int.zero) return;

            float targetZAngle = 0f;

            if (dir == Vector2Int.up) targetZAngle = 0f;
            else if (dir == Vector2Int.left) targetZAngle = 90f;
            else if (dir == Vector2Int.down) targetZAngle = 180f;
            else if (dir == Vector2Int.right) targetZAngle = -90f;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZAngle);

            if (LeftEye != null) LeftEye.transform.localRotation = targetRotation;
            if (RightEye != null) RightEye.transform.localRotation = targetRotation;
        }
    }
}