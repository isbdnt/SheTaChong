using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SheTaChong
{
    public class Grid
    {
        public Tile this[Vector2Int pos]
        {
            get
            {
                try
                {
                    return _tiles[pos.x, pos.y];
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
                    _tiles[pos.x, pos.y] = value;
                }
                catch (Exception)
                {
                    Debug.Log(pos);
                }
            }
        }
        protected Tile[,] _tiles;

        public int width => _gridData.width;
        public int height => _gridData.height;
        public int aim => _gridData.aim;
        public List<GridRecord> gridRecords { get; } = new List<GridRecord>();
        public LinkedList<Tile> snake { get; private set; } = new LinkedList<Tile>();
        public GridStateType state { get; private set; } = GridStateType.Playable;
        public int points { get; private set; }

        GridData _gridData;

        public Grid(GridData gridData)
        {
            _gridData = gridData;
            _tiles = new Tile[_gridData.width, _gridData.height];
            foreach (var tileData in _gridData.tiles)
            {
                _tiles[tileData.position.x, tileData.position.y] = new Tile(tileData);
            }
            foreach (var pos in _gridData.snake.positions)
            {
                Tile tile = _tiles[pos.x, pos.y];
                tile.type = TileType.Snake;
                snake.AddLast(tile);
            }
        }

        public void SnakeMove(Vector2Int dir)
        {
            Tile nextTile = GetSnakeMoveNextTile(dir);
            if (nextTile != null)
            {
                SnakeTryMoveTo(nextTile);
            }
        }

        Tile GetSnakeMoveNextTile(Vector2Int dir)
        {
            if (dir.x != 0)
            {
                Vector2Int pos = snake.First.Value.position;
                if (dir.x > 0)
                {
                    if (_tiles[pos.x, pos.y].HasRightBorder() == false)
                    {
                        return _tiles[pos.x + 1, pos.y];
                    }
                }
                else
                {
                    if (_tiles[pos.x, pos.y].HasLeftBorder() == false)
                    {
                        return _tiles[pos.x - 1, pos.y];
                    }
                }
            }
            else if (dir.y != 0)
            {
                Vector2Int pos = snake.First.Value.position;
                if (dir.y > 0)
                {
                    if (_tiles[pos.x, pos.y].HasTopBorder() == false)
                    {
                        return _tiles[pos.x, pos.y + 1];
                    }
                }
                else
                {
                    if (_tiles[pos.x, pos.y].HasBottomBorder() == false)
                    {
                        return _tiles[pos.x, pos.y - 1];
                    }
                }
            }
            return null;
        }
        void SnakeTryMoveTo(Tile nextTile)
        {
            switch (nextTile.type)
            {
                case TileType.Empty:
                    SnakeMoveTo(nextTile);
                    LoseIfCantMove();
                    break;
                case TileType.Exit:
                    SnakeMoveTo(nextTile);
                    if (points >= aim)
                    {
                        gridRecords.Add(new GridRecord()
                        {
                            recordType = GridRecordType.Win,
                        });
                        state = GridStateType.Win;
                    }
                    else
                    {
                        gridRecords.Add(new GridRecord()
                        {
                            recordType = GridRecordType.Lose,
                        });
                        state = GridStateType.Lose;
                    }
                    break;
                case TileType.Snake:
                    break;
                case TileType.Mouse:
                    points++;
                    gridRecords.Add(new GridRecord()
                    {
                        recordType = GridRecordType.Eaten,
                        tilePositions = new List<Vector2Int>() { nextTile.position },
                    });
                    gridRecords.Add(new GridRecord()
                    {
                        recordType = GridRecordType.SnakeAddBody,
                    });
                    SnakeMoveTo(nextTile, true);
                    LoseIfCantMove();
                    break;
                case TileType.Frog:
                    points++;
                    gridRecords.Add(new GridRecord()
                    {
                        recordType = GridRecordType.Eaten,
                        tilePositions = new List<Vector2Int>() { nextTile.position },
                    });
                    gridRecords.Add(new GridRecord()
                    {
                        recordType = GridRecordType.SnakeAddBody,
                    });
                    SnakeMoveTo(nextTile, true);
                    gridRecords.Add(new GridRecord()
                    {
                        recordType = GridRecordType.SnakeReverse,
                    });
                    var reversedSnake = new LinkedList<Tile>();
                    LinkedListNode<Tile> node = snake.Last;
                    while (node != null)
                    {
                        reversedSnake.AddLast(node.Value);
                        node = node.Previous;
                    }
                    snake = reversedSnake;
                    LoseIfCantMove();
                    break;
                default:
                    break;
            }
        }

        bool CanSnakeMove(Vector2Int dir)
        {
            Tile nextTile = GetSnakeMoveNextTile(dir);
            if (nextTile == null)
            {
                return false;
            }
            switch (nextTile.type)
            {
                case TileType.Exit:
                    return points >= aim;
                case TileType.Snake:
                    return false;
                default:
                    return true;
            }
        }

        void LoseIfCantMove()
        {
            if (CanSnakeMove(Vector2Int.down) == false && CanSnakeMove(Vector2Int.up) == false && CanSnakeMove(Vector2Int.left) == false && CanSnakeMove(Vector2Int.right) == false)
            {
                gridRecords.Add(new GridRecord()
                {
                    recordType = GridRecordType.Lose,
                });
                state = GridStateType.Lose;
            }
        }

        void SnakeMoveTo(Tile nextTile, bool addBody = false)
        {
            nextTile.type = TileType.Snake;
            snake.AddFirst(nextTile);
            if (addBody == false)
            {
                snake.Last.Value.type = TileType.Empty;
                snake.RemoveLast();
            }
            gridRecords.Add(new GridRecord()
            {
                recordType = GridRecordType.SnakeMove,
                tilePositions = snake.Select(t => t.position).ToList(),
            });
        }

        public void Reset()
        {
            gridRecords.Clear();
            foreach (var tileData in _gridData.tiles)
            {
                _tiles[tileData.position.x, tileData.position.y].Reset();
            }
            snake.Clear();
            foreach (var pos in _gridData.snake.positions)
            {
                Tile tile = _tiles[pos.x, pos.y];
                tile.type = TileType.Snake;
                snake.AddLast(tile);
            }
            state = GridStateType.Playable;
        }
    }
}
