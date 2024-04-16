using BeABachelor.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneFadeController : FadeController
    {
        [Inject] private PlaySceneManager _playSceneManager;
        
        private void Awake()
        {
            Image = GetComponent<Image>();
            Image.enabled = true;
            Animator = GetComponent<Animator>();
            FadeOut = Animator.StringToHash("FadeOut");
            _playSceneManager.PlayFadeIn = PlayFadeIn;
            _playSceneManager.PlayFadeOut = PlayFadeOut;
        }
    }
}