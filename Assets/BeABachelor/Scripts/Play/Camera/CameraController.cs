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
        }

        private void OnDestroy()
        {
        }

        private void SetTarget(GameState gameState)
        {
            if (gameState == GameState.CountDown)
            {
                var target = _playSceneManager.GetPlayerObject();

                virtualCamera.m_Follow = target.transform;
                if (target.tag == "Hakken")
                {
                    cameraOffset.m_Offset = new Vector3(cameraOffset.m_Offset.x, cameraOffset.m_Offset.y,
                        -cameraOffset.m_Offset.z);
                }
            }
        }
    }
}