using BeABachelor.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAudioManager : MonoBehaviour, IAudioManager
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _confirmSE;

    public void PlayConfirmSE()
    {
        _audioSource?.PlayOneShot(_confirmSE);
    }


    public AudioSource GetAudioSource()
    {
        return _audioSource;
    }
}
