using System;
using BeABachelor.Interface;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace BeABachelor.Play.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera followCamera;
        [SerializeField] private CinemachineVirtualCamera focusCamera;

        [Inject] private IGameManager _gameManager;
        [Inject] private PlaySceneManager _playSceneManager;

        private void Awake()
        {
            followCamera.Priority = 10;
            focusCamera.Priority = 20;
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

                followCamera.m_Follow = target.transform;
                focusCamera.m_Follow = target.transform;
                focusCamera.m_LookAt = target.transform;

                if (target.tag == "Hakken")
                {
                    // とりあえずカメラ座標全体を反転
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                // カメラ切り替え
                focusCamera.Priority = 1;
            }
        }
    }
}