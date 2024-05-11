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

        public void PlayItemSE()
        {
            _seAudioSource?.PlayOneShot(_itemSE);
        }

        public AudioSource GetAudioSource()
        {
            return _seAudioSource;
        }
    }
}
