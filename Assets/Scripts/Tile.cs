using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SheTaChong
{
    public class Tile
    {
        public TileType type;
        public Vector2Int position => _data.position;

        TileData _data;

        public Tile(TileData tileData)
        {
            _data = tileData;
            type = tileData.type;
        }

        public void Reset()
        {
            type = _data.type;
        }

        public bool HasTopBorder()
        {
            return (_data.border & 2) != 0;
        }

        public bool HasRightBorder()
        {
            return (_data.border & 4) != 0;
        }

        public bool HasBottomBorder()
        {
            return (_data.border & 8) != 0;
        }

        public bool HasLeftBorder()
        {
            return (_data.border & 16) != 0;
        }
    }
}
