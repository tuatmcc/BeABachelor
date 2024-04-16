using System;
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
            Animator.SetBool(FadeOut, true);
        }
    }
}
