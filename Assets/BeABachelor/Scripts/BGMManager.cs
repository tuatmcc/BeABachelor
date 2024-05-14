using BeABachelor;
using BeABachelor.Interface;
using BeABachelor.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BeABachelor
{
    public class BGMManager : MonoBehaviour
    {
        [SerializeField] private AudioClip menuClip;
        [SerializeField] private AudioClip playingClip;

        [Inject] IGameManager _gameManager;
        [Inject] AudioSource _bgmAudioSource;

        private void Awake()
        {
            _gameManager.OnGameStateChanged += OnGameStateChanged;
            _bgmAudioSource.loop = true;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Title:
                    _bgmAudioSource.Stop();
                    AudioFader.AudioFadeIn(_bgmAudioSource, menuClip, 0.1f);
                    break;
                case GameState.Ready:
                    AudioFader.AudioFadeOut(_bgmAudioSource, 0.2f);
                    break;
                case GameState.Playing:
                    AudioFader.AudioFadeIn(_bgmAudioSource, playingClip, 0.05f);
                    break;
                case GameState.Finished:
                    AudioFader.AudioFadeOut(_bgmAudioSource, 0.05f);
                    break;
            }
        }

    }
}
