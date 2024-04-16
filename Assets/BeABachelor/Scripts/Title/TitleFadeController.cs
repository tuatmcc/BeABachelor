using BeABachelor.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Title
{
    public class TitleFadeController : FadeController
    {
        [Inject] private ITitleManager _titleManager;
        private void Awake()
        {
            Image = GetComponent<Image>();
            Image.enabled = true;
            Animator = GetComponent<Animator>();
            FadeOut = Animator.StringToHash("FadeOut");
            _titleManager.PlayFadeIn = PlayFadeIn;
            _titleManager.PlayFadeOut = PlayFadeOut;
        }
    }
}