using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SheTaChong.Unity
{
    public class UserTouchScreenGridInput : MonoBehaviour, IGridInput, IBeginDragHandler, IDragHandler,IEndDragHandler
    {
        public Vector2Int position { get; private set; }
        public Vector2Int direction { get; private set; }

        GridView _gridView;
        Vector2Int _dir;
        void Awake()
        {
            _gridView = GetComponent<GridView>();
            _gridView.input = this;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 pos = (Camera.main.ScreenToWorldPoint(eventData.pressPosition) - _gridView.pivot) / _gridView.cellSize;
            position = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            Vector2 dirNorm = eventData.delta.normalized;
            _dir = new Vector2Int(Mathf.RoundToInt(dirNorm.x), Mathf.RoundToInt(dirNorm.y));
        }

        public void Use()
        {
            position = Vector2Int.zero;
            direction = Vector2Int.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            direction = _dir;
            _dir = Vector2Int.zero;
        }
    }
}
