using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SheTaChong.Unity
{
    public class GridView : MonoBehaviour
    {
        public TileView this[Vector2Int pos]
        {
            get
            {
                try
                {
                    return _tileViews[pos.x, pos.y];
                }
                catch (Exception)
                {
                    Debug.Log(pos);
                    return null;
                }
            }
            set
            {
                try
                {
                    _tileViews[pos.x, pos.y] = value;
                }
                catch (Exception)
                {
                    Debug.Log(pos);
                }
            }
        }
        public Vector3 pivot { get; private set; }
        public float cellSize;
        public IGridInput input;
        public Grid grid { get; private set; }

        [SerializeField]
        GameObject _tilePrefab;
        [SerializeField]
        GameObject _snakePrefab;
        [SerializeField]
        int _maxLevel;
        [SerializeField]
        GameObject _clearAllTip;
        TileView[,] _tileViews;
        SnakeView _snakeView;

        Queue<List<GridRecord>> _gridRecordsQueue = new Queue<List<GridRecord>>();
        int _animCount;

        private void Start()
        {
            if (PlayerPrefs.HasKey("currentLevel") == false)
            {
                PlayerPrefs.SetInt("currentLevel", 1);
            }
            if (PlayerPrefs.HasKey("unlockLevel") == false)
            {
                PlayerPrefs.SetInt("unlockLevel", 1);
            }
            ResetInternalState();
            Initialize();
        }

        void Initialize()
        {
            CreateGrid(PlayerPrefs.GetInt("currentLevel"));
            pivot = new Vector3((0.5f - grid.width / 2f) * cellSize, (0.5f - grid.height / 2f) * cellSize);
            _tileViews = new TileView[grid.width, grid.height];
            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    Tile tile = grid[new Vector2Int(x, y)];
                    if (tile != null)
                    {
                        _tileViews[x, y] = CreateTileView(tile);
                    }
                }
            }
            _snakeView = GameObject.Instantiate(_snakePrefab, transform).GetComponent<SnakeView>();
            _snakeView.Initialize(grid.snake.Select(t => t.position).ToList(), cellSize, pivot);
        }

        public TileView CreateTileView(Tile tile)
        {
            var tileView = GameObject.Instantiate(_tilePrefab).GetComponent<TileView>();
            tileView.Initialize(tile, cellSize, pivot, transform);
            return tileView;
        }

        private void FixedUpdate()
        {
            if (grid.state == GridStateType.Playable)
            {
                if (input.direction != Vector2Int.zero)
                {
                    grid.SnakeMove(input.direction);
                    input.Use();
                    if (grid.gridRecords.Count > 0)
                    {
                        _gridRecordsQueue.Enqueue(new List<GridRecord>(grid.gridRecords));
                        grid.gridRecords.Clear();
                    }
                }
            }
            else
            {
                if (grid.state == GridStateType.Win && _animCount == 0)
                {
                    Win();
                }
            }

            if (_animCount == 0 && _gridRecordsQueue.Count > 0)
            {
                foreach (var gridRecord in _gridRecordsQueue.Dequeue())
                {
                    switch (gridRecord.recordType)
                    {
                        case GridRecordType.SnakeMove:
                            _animCount++;
                            _snakeView.Move(gridRecord.tilePositions, ReduceAnimationCount);
                            break;
                        case GridRecordType.SnakeAddBody:
                            _animCount++;
                            _snakeView.AddBody(ReduceAnimationCount);
                            break;
                        case GridRecordType.SnakeReverse:
                            _animCount++;
                            _snakeView.Reverse(ReduceAnimationCount);
                            break;
                        case GridRecordType.Eaten:
                            _animCount++;
                            Vector2Int pos = gridRecord.tilePositions[0];
                            _tileViews[pos.x, pos.y].Eaten(ReduceAnimationCount);
                            break;
                        case GridRecordType.Lose:
                            _animCount++;
                            _snakeView.Lose(ReduceAnimationCount);
                            break;
                        case GridRecordType.Win:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        void Win()
        {
            if (PlayerPrefs.GetInt("currentLevel") == PlayerPrefs.GetInt("unlockLevel"))
            {
                PlayerPrefs.SetInt("unlockLevel", PlayerPrefs.GetInt("unlockLevel") + 1);
            }
            if (PlayerPrefs.GetInt("currentLevel") == _maxLevel)
            {
                _clearAllTip.SetActive(true);
            }
            NextLevel();
        }

        void ReduceAnimationCount()
        {
            _animCount--;
        }

        public void Restart()
        {
            ResetInternalState();
            grid.Reset();
            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    _tileViews[x, y]?.ResetView();
                }
            }
            _snakeView.ResetView(grid.snake.Select(t => t.position).ToList());
        }

        void ResetInternalState()
        {
            _animCount = 0;
            _gridRecordsQueue.Clear();
            input.Use();
            _clearAllTip.SetActive(false);
        }


        public void PreviousLevel()
        {
            if (PlayerPrefs.GetInt("currentLevel") > 1)
            {
                SelectLevel(PlayerPrefs.GetInt("currentLevel") - 1);
            }
        }

        public void NextLevel()
        {
            int currentLevel = PlayerPrefs.GetInt("currentLevel");
            if (currentLevel < _maxLevel && currentLevel < PlayerPrefs.GetInt("unlockLevel"))
            {
                SelectLevel(PlayerPrefs.GetInt("currentLevel") + 1);
            }
        }

        void SelectLevel(int level)
        {
            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    if (_tileViews[x, y] != null)
                    {
                        GameObject.Destroy(_tileViews[x, y].gameObject);
                        _tileViews[x, y] = null;
                    }
                }
            }
            GameObject.Destroy(_snakeView.gameObject);
            PlayerPrefs.SetInt("currentLevel", level);
            ResetInternalState();
            Initialize();
        }

        void CreateGrid(int level)
        {
            PlayerPrefs.SetInt("currentLevel", level);
            var gridData = JsonConvert.DeserializeObject<GridData>(Resources.Load<TextAsset>($"Configs/Levels/{level}").text);
            grid = new Grid(gridData);
        }
    }
}
