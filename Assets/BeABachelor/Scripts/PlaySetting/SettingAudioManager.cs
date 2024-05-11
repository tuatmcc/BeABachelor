using BeABachelor.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeABachelor.PlaySetting
{
    public class SettingAudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip selectSE;

        public void PlaySelectSE()
        {
            audioSource?.PlayOneShot(selectSE);
        }

        public AudioSource GetAudioSource()
        {
            return audioSource;
        }
    }
}
