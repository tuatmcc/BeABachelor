using System;
using BeABachelor.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.UI.PlayerPos
{
    public class PlayerPosUIController : MonoBehaviour
    {
        [SerializeField] private GameObject arrow;
        [SerializeField] private GameObject text;
        [SerializeField] private RectTransform canvasRect;
        
        private RectTransform _arrowRect;
        private Transform _targetPlayerTransform;
        
        [Inject] private IGameManager _gameManager;
        [Inject] private PlaySceneManager _playSceneManager;
        private readonly Vector2 _offset = new(-960.0f, 0f);
        
        private void Start()
        {
            _arrowRect = arrow.GetComponent<RectTransform>();
            _targetPlayerTransform = _gameManager.PlayerType == PlayerType.Hakken ?
                _playSceneManager.Kouken.transform : _playSceneManager.Hakken.transform;
        }

        private void FixedUpdate()
        {
            Vector2 pos;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_playSceneManager.MainCamera, _targetPlayerTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out pos);
            _arrowRect.anchoredPosition = pos / 9.0f + _offset;
        }
    }
}