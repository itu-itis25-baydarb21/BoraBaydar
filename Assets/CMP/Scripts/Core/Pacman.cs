using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CMP.Scripts
{
    public class Pacman : MonoBehaviour
    {
        public InputManager InputManager;
        private Direction _currentMoveDirection = Direction.None;
        private Direction _desiredMoveDirection = Direction.None;
        public Animator Animator;
        public const string FailTriggerName = "Fail";
        private const string FailAnimationName = "FailAnimation";
        public float Speed;
        [SerializeField] private GameObject pacmanVisual;
        [SerializeField] private GridData gridData;
        private List<Vector2Int> _notMoveableCellCords = new List<Vector2Int>();
        private Vector2Int _targetPosition;
        private HashSet<Vector2Int> _notMovableCellCordsSet;
        

        private void Awake()
        {
            Speed = 1f / GameSettings.PacmanMovementDuration;

            if(Animator == null)
            {
                Animator = GetComponentInChildren<Animator>();
            }
        }
        private void Start() 
        {
            if (InputManager == null) 
            {
                InputManager = FindObjectOfType<InputManager>();
            }

            GetMoveableCellCords();
            _notMovableCellCordsSet = new HashSet<Vector2Int>(_notMoveableCellCords);
            _targetPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }
        private void Update()
        {
            Direction newInput = InputManager.ConsumeInput();

            if(newInput != Direction.None) 
            {
                _desiredMoveDirection = newInput; 
            }
            MovePacman();
        }

        private void GetMoveableCellCords()
        {
            _notMoveableCellCords.AddRange(gridData.GetCoordsOfCellType(CellType.Wall));
            _notMoveableCellCords.AddRange(gridData.GetCoordsOfCellType(CellType.AiSpawnZone));
            _notMoveableCellCords.AddRange(gridData.GetCoordsOfCellType(CellType.AiGate));
        }

        public void PlayFailAnimation()
        {
            this.enabled = false;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopWaka();
                Animator.speed = 1f; 
                Animator.SetTrigger(FailTriggerName);
            }
        }

        private void UpdateFacingDirection() 
        {
            switch(_currentMoveDirection) 
            {
                case Direction.Right:
                    transform.rotation = Quaternion.Euler(0,0,0);
                    pacmanVisual.transform.rotation = Quaternion.Euler(0,0,0);
                    break;
                case Direction.Up:
                    transform.rotation = Quaternion.Euler(0,0,90);
                    pacmanVisual.transform.rotation = Quaternion.Euler(0,0,90);
                    break;
                case Direction.Left:
                    transform.rotation = Quaternion.Euler(0,0,180);
                    pacmanVisual.transform.rotation = Quaternion.Euler(0,0,180);
                    break;
                case Direction.Down:
                    transform.rotation = Quaternion.Euler(0,0,270);
                    pacmanVisual.transform.rotation = Quaternion.Euler(0,0,270);
                    break;
            }
        }

        private void MovePacman()
        {
            if (_currentMoveDirection == Direction.None && _desiredMoveDirection == Direction.None)
            {
                if (Animator != null) Animator.speed = 0f;
                if (AudioManager.Instance != null) AudioManager.Instance.StopWaka();
                return;
            }

            Vector3 finalTargetPosition = (Vector3)(Vector2)_targetPosition;
            bool isAtCenter = Vector3.Distance(transform.position, finalTargetPosition) < 0.01f;

            if (isAtCenter)
            {
                Vector2Int pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                Vector2Int desiredVector = _desiredMoveDirection.ToVector2Int();
                Vector2Int desiredNextCell = pos + desiredVector;

                bool isDesiredValid = desiredNextCell.x >= 0 && desiredNextCell.x < gridData.Width &&
                                    desiredNextCell.y >= 0 && desiredNextCell.y < gridData.Height &&
                                    !_notMovableCellCordsSet.Contains(desiredNextCell);

                if (_desiredMoveDirection != Direction.None && isDesiredValid)
                {
                    _currentMoveDirection = _desiredMoveDirection;
                    UpdateFacingDirection();
                    _targetPosition = desiredNextCell;
                }
                else
                {
                    Vector2Int currentVector = _currentMoveDirection.ToVector2Int();
                    Vector2Int currentNextCell = pos + currentVector;

                    bool isCurrentValid = currentNextCell.x >= 0 && currentNextCell.x < gridData.Width &&
                                        currentNextCell.y >= 0 && currentNextCell.y < gridData.Height &&
                                        !_notMovableCellCordsSet.Contains(currentNextCell);

                    if (isCurrentValid)
                    {
                        _targetPosition = currentNextCell;
                    }
                    else
                    {
                        _targetPosition = pos;
                        _currentMoveDirection = Direction.None;
                    }
                }
                finalTargetPosition = (Vector3)(Vector2)_targetPosition;
            }

            Vector3 prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, finalTargetPosition, Speed * Time.deltaTime);

            bool isActuallyMoving = (transform.position - prevPosition).sqrMagnitude > 0.00001f;

            if (Animator != null)
            {
                Animator.speed = isActuallyMoving ? 1f : 0f;
            }

            if (AudioManager.Instance != null)
            {
                if (isActuallyMoving)
                {
                    AudioManager.Instance.StartWaka();
                }
                else
                {
                    AudioManager.Instance.StopWaka();
                }
            }
        }
    }
}