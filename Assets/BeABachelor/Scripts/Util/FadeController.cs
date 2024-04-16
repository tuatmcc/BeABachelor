using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Util
{
    public abstract class FadeController : MonoBehaviour
    {
        protected Animator Animator;
        protected int FadeOut;
        protected Image Image;

        public void PlayFadeIn()
        {
            Animator.SetBool(FadeOut, false);
        }

        public void PlayFadeOut()
        {
            Image.enabled = true;
            Animator.SetBool(FadeOut, true);
        }

        protected void DisableImage()
        {
            DisableImageAsync().Forget();
        }
        
        private async UniTask DisableImageAsync()
        {
            await UniTask.Delay(500);
            Image.enabled = false;
        }
    }
}
