using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace SheTaChong.Unity
{
    public class SnakeView : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer _headRenderer;
        [SerializeField]
        Sprite _headSprite;
        [SerializeField]
        Sprite _deadHeadSprite;
        [SerializeField]
        GameObject _bodyTemplate;
        List<SpriteRenderer> _snake = new List<SpriteRenderer>();
        List<SpriteRenderer> _addedBody = new List<SpriteRenderer>();
        List<Vector2Int> _rawPositions;
        Vector3 _pivot;
        float _size;

        public void Initialize(List<Vector2Int> positions, float size, Vector3 pivot)
        {
            _rawPositions = positions;
            _pivot = pivot;
            _size = size;
            _bodyTemplate.SetActive(true);
            for (int i = 0; i < _rawPositions.Count; i++)
            {
                if (i == 0)
                {
                    _snake.Add(_headRenderer);
                    _snake[i].transform.position = _pivot + new Vector3(_rawPositions[i].x, _rawPositions[i].y) * _size;
                    _snake[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(_rawPositions[i] - _rawPositions[i + 1]));
                }
                else
                {
                    _snake.Add(GameObject.Instantiate(_bodyTemplate, transform, true).GetComponent<SpriteRenderer>());
                    _snake[_snake.Count - 1].sortingOrder = _snake[_snake.Count - 2].sortingOrder - 1;
                    var centerPos = (Vector2)(_rawPositions[i - 1] + _rawPositions[i]) / 2;
                    _snake[i].transform.position = _pivot + new Vector3(centerPos.x, centerPos.y) * _size;
                    _snake[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(_rawPositions[i - 1] - _rawPositions[i]));
                }
            }
            _bodyTemplate.SetActive(false);
        }

        public void Move(List<Vector2Int> positions, Action onEnd)
        {
            _rawPositions = positions;
            for (int i = 0; i < _rawPositions.Count; i++)
            {
                if (i == 0)
                {
                    _snake[i].transform.position = _pivot + new Vector3(_rawPositions[i].x, _rawPositions[i].y) * _size;
                    _snake[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(_rawPositions[i] - _rawPositions[i + 1]));
                }
                else
                {
                    var centerPos = (Vector2)(_rawPositions[i - 1] + _rawPositions[i]) / 2;
                    _snake[i].transform.position = _pivot + new Vector3(centerPos.x, centerPos.y) * _size;
                    _snake[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(_rawPositions[i - 1] - _rawPositions[i]));
                }
            }
            onEnd();
        }

        public void AddBody(Action onEnd)
        {
            _snake.Add(GameObject.Instantiate(_snake[_snake.Count - 1], transform, true).GetComponent<SpriteRenderer>());
            _snake[_snake.Count - 1].sortingOrder = _snake[_snake.Count - 2].sortingOrder - 1;
            _addedBody.Add(_snake[_snake.Count - 1]);
            onEnd();
        }

        public void Reverse(Action onEnd)
        {
            _rawPositions.Reverse();
            Move(_rawPositions, onEnd);
        }

        public void ResetView(List<Vector2Int> positions)
        {
            foreach (var body in _addedBody)
            {
                _snake.Remove(body);
                GameObject.Destroy(body.gameObject);
            }
            _addedBody.Clear();
            for (int i = 0; i < positions.Count; i++)
            {
                if (i == 0)
                {
                    _snake[i].transform.position = _pivot + new Vector3(positions[i].x, positions[i].y) * _size;
                    _snake[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(positions[i] - positions[i + 1]));
                }
                else
                {
                    var centerPos = (Vector2)(positions[i - 1] + positions[i]) / 2;
                    _snake[i].transform.position = _pivot + new Vector3(centerPos.x, centerPos.y) * _size;
                    _snake[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(positions[i - 1] - positions[i]));
                }
            }
            _headRenderer.sprite = _headSprite;
        }

        public void Lose(Action onEnd)
        {
            _headRenderer.sprite = _deadHeadSprite;
            onEnd();
        }
    }
}
