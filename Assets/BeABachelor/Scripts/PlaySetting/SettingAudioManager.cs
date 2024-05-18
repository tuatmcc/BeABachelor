using BeABachelor.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeABachelor.PlaySetting
{
    public class SettingAudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip _selectSE;
        [SerializeField] AudioClip _confirmSE;

        public void PlaySelectSE()
        {
            _audioSource?.PlayOneShot(_selectSE);
        }

        public void playConfirmSE()
        {
            _audioSource?.PlayOneShot(_confirmSE);
        }

        public AudioSource GetAudioSource()
        {
            return _audioSource;
        }
    }
}
