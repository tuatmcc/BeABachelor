using BeABachelor.Interface;
using BeABachelor.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneAudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] private AudioSource _seAudioSource;
        [SerializeField] private AudioClip _itemSE;
        [SerializeField] private AudioClip _countSE;
        [SerializeField] private AudioClip _startSE;

        [Inject] private PlaySceneManager _playSceneManager;

        private void Awake()
        {
            _playSceneManager.OnCountChanged += PlayCountSE;
        }

        public void PlayItemSE()
        {
            _seAudioSource?.PlayOneShot(_itemSE);
        }

        private void PlayCountSE(int count)
        {
            if(count > 0)
            {
                _seAudioSource?.PlayOneShot(_countSE);
            }
            else
            {
                _seAudioSource?.PlayOneShot(_startSE);
            }
        }

        public AudioSource GetAudioSource()
        {
            return _seAudioSource;
        }
    }
}
