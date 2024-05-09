using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeABachelor.Play
{
    public class PlaySceneAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _playSceneAudioSource;
        [SerializeField] private AudioClip _itemSE;

        public void PlayItemSE()
        {
            _playSceneAudioSource?.PlayOneShot(_itemSE);
        }

        public AudioSource GetAudioSource()
        {
            return _playSceneAudioSource;
        }
    }
}
