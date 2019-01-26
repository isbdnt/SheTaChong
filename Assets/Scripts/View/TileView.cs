using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SheTaChong.Unity
{
    public class TileView : MonoBehaviour
    {
        [SerializeField]
        Sprite[] _sprites;
        [SerializeField]
        SpriteRenderer _spriteRenderer;
        [SerializeField]
        SpriteRenderer _topBorder;
        [SerializeField]
        SpriteRenderer _rightBorder;
        [SerializeField]
        SpriteRenderer _bottomBorder;
        [SerializeField]
        SpriteRenderer _leftBorder;
        Tile _tile;

        public void Initialize(Tile tile, float size, Vector3 pivot, Transform parent)
        {
            _tile = tile;
            transform.localPosition = pivot + new Vector3(_tile.position.x, _tile.position.y) * size;
            transform.SetParent(parent);
            _spriteRenderer.sprite = _sprites[(int)_tile.type];
            _topBorder.gameObject.SetActive(_tile.HasTopBorder());
            _rightBorder.gameObject.SetActive(_tile.HasRightBorder());
            _bottomBorder.gameObject.SetActive(_tile.HasBottomBorder());
            _leftBorder.gameObject.SetActive(_tile.HasLeftBorder());
        }

        public void Eaten(Action onEnd)
        {
            _spriteRenderer.sprite = _sprites[(int)TileType.Empty];
            onEnd();
        }

        public void ResetView()
        {
            _spriteRenderer.sprite = _sprites[(int)_tile.type];
        }
    }
}
