using System.Collections.Generic;
using CMP.Scripts.AiStates;
using UnityEngine;

using GhostStateBase = CMP.Scripts.AiStates.GhostState;

namespace CMP.Scripts
{
    public enum GhostState
    {
        InHouse,
        JoiningGame,
        Scatter,
        Chase,
    }

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
                Speed = 3f,
                CurrentGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)),
                TargetGridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y))
            };

            InHouseState = new InHouseState(Blackboard);
            ChangeState(InHouseState);
        }

        public void Update()
        {
            CurrentState?.Update();
        }

        public void ChangeState(GhostStateBase newState)
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }
    }
}