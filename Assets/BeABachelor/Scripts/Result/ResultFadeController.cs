using BeABachelor.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultFadeController : FadeController
    {
        [Inject] private IResultManager _resultManager;
        
        private void Awake()
        {
            Image = GetComponent<Image>();
            Image.enabled = true;
            Animator = GetComponent<Animator>();
            FadeOut = Animator.StringToHash("FadeOut");
            _resultManager.PlayFadeIn = PlayFadeIn;
            _resultManager.PlayFadeOut = PlayFadeOut;
            DisableImage();
        }
    }
}