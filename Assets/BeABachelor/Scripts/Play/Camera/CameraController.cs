using System;
using BeABachelor.Interface;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CinemachineCameraOffset cameraOffset;

        [Inject] private IGameManager _gameManager;
        [Inject] private PlaySceneManager _playSceneManager;

        private void Awake()
        {
            _gameManager.OnGameStateChanged += SetTarget;
        }

        private void OnDestroy()
        {
            _gameManager.OnGameStateChanged -= SetTarget;
        }

        private void SetTarget(GameState gameState)
        {
            if (gameState == GameState.CountDown)
            {
                var target = _playSceneManager.GetPlayerObject();

                virtualCamera.m_Follow = target.transform;

                if (target.tag == "Hakken")
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
        }
    }
}