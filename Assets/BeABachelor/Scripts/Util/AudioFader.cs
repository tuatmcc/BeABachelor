using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeABachelor.Util
{
    public static class AudioFader
    {
        public static void AudioFadeIn(AudioSource audioSource, AudioClip audioClip, float fadeSpeed)
        {
            audioSource.clip = audioClip;
            audioSource.volume = 0;
            audioSource.Play();
            VolumeUp(audioSource, fadeSpeed).Forget();
        }
    
        public static void AudioFadeOut(AudioSource audioSource, float fadeSpeed)
        {
            VolumeDown(audioSource, fadeSpeed).Forget();
        }

        private static async UniTask VolumeUp(AudioSource audioSource, float fadeSpeed)
        {
            while(audioSource != null && audioSource.volume < 1f)
            {
                await UniTask.WaitForSeconds(0.1f);
                audioSource.volume = Mathf.Min(audioSource.volume + fadeSpeed, 1f);
            }
        }

        private static async UniTask VolumeDown(AudioSource audioSource, float fadeSpeed)
        {
            while (audioSource != null && audioSource.volume > 0f)
            {
                await UniTask.WaitForSeconds(0.1f);
                audioSource.volume = Mathf.Max(audioSource.volume - fadeSpeed, 0f);
            }
            audioSource?.Stop();
        }
    }
}   
