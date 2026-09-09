using System.Collections.Generic;
using CMP.Scripts.AiStates;
using CMP.Scripts.Helper;
using UnityEngine;

namespace CMP.Scripts
{
    public enum GameMode
    {
        Scatter,
        Chase,
        GameOver,
    }

    public class GameManager : MonoBehaviour
    {
        private Pacman _pacman;
        private InputManager _inputManager;
        private GameMode _gameMode = GameMode.Scatter;
        private readonly List<Ghost> _ghosts = new();
        private List<Vector2Int> _ghostSpawnPoints;
        private List<Vector2Int> _pacmanSpawnPoint;

        [SerializeField] private GridData gridData;

        public GameMode CurrentGameMode => _gameMode;

        private void Start()
        {
            gridData = AssetDatabase.Instance.GridData;
            _pacman = Instantiate(AssetDatabase.Instance.PacmanPrefab);
            _inputManager = Instantiate(AssetDatabase.Instance.InputManagerPrefab);

            for (int i = 0; i < GameSettings.AiCharacterCount; i++)
            {
                Ghost newGhost = Instantiate(AssetDatabase.Instance.Ghost);
                _ghosts.Add(newGhost);
            }

            CreateBackground(gridData);
            AdjustCamera(gridData);

            GetPacmanSpawnPoint();
            AdjustPacmanSpawnPoint();

            GetGhostSpawnPoints();
            AdjustGhostSpawnPoints();
        }

        private void CreateBackground(GridData data)
        {
            var targetTexture = MapTextureGenerator.Generate(data, AssetDatabase.Instance.MapVisualSettings);
            var textureObject = new GameObject("MapTexture");
            textureObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);

            var targetSprite = Sprite.Create(
                targetTexture, 
                new Rect(0f, 0f, targetTexture.width, targetTexture.height),
                Vector2.zero, 
                AssetDatabase.Instance.MapVisualSettings.pixelsPerCell
            );

            var spriteRenderer = textureObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = targetSprite;
            spriteRenderer.sortingOrder = -1;
        }

        private void AdjustCamera(GridData data)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null) return;

            mainCamera.orthographicSize = data.Height + GameSettings.CameraPadding;
            mainCamera.transform.position = new Vector3(data.Width / 2f - 0.5f, 0f, -10f);
        }

        private void GetPacmanSpawnPoint()
        {
            _pacmanSpawnPoint = gridData.GetCoordsOfCellType(CellType.Pacman);
        }

        private void AdjustPacmanSpawnPoint()
        {
            if (_pacmanSpawnPoint != null && _pacmanSpawnPoint.Count > 0)
            {
                _pacman.transform.position = (Vector3)(Vector2)_pacmanSpawnPoint[0];
            }
        }

        private void GetGhostSpawnPoints()
        {
            _ghostSpawnPoints = gridData.GetCoordsOfCellType(CellType.AiSpawnZone);
        }

        private void AdjustGhostSpawnPoints()
        {
            for (int i = 0; i < _ghosts.Count; i++)
            {
                if (i < _ghostSpawnPoints.Count)
                {
                    _ghosts[i].transform.position = (Vector3)(Vector2)_ghostSpawnPoints[i];

                    float delay = (GameSettings.AiJoinDelays != null && i < GameSettings.AiJoinDelays.Length)
                        ? GameSettings.AiJoinDelays[i]
                        : (i + 1) * 3f;

                    _ghosts[i].Initialize(gridData, _pacman.transform, this, delay);
                }
                else
                {
                    Debug.LogWarning($"Can't found enough AiSpawnZone for the ghosts {i}");
                }
            }
        }

        public void TriggerAllGhostsChase()
        {
            if (_gameMode == GameMode.GameOver || _gameMode == GameMode.Chase) return;

            _gameMode = GameMode.Chase;

            foreach (var ghost in _ghosts)
            {
                if (ghost.CurrentState is ScatterState)
                {
                    ghost.ChangeState(new ChaseState(ghost.Blackboard));
                }
            }
        }

        public void TriggerGameOver()
        {
            if (_gameMode == GameMode.GameOver) return;

            _gameMode = GameMode.GameOver;

            if (_pacman != null)
            {
                _pacman.enabled = false;
            }

            if (_inputManager != null)
            {
                _inputManager.enabled = false;
            }

            foreach (var ghost in _ghosts)
            {
                ghost.enabled = false;
            }
        }
    }
}