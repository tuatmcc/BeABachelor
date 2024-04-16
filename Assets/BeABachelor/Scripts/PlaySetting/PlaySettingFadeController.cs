using BeABachelor.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = System.Object;

namespace BeABachelor.PlaySetting
{
    public class PlaySettingFadeController : FadeController
    {
        [Inject] private IPlaySettingManager _playSettingManager;
        
        private void Awake()
        {
            Image = GetComponent<Image>();
            Image.enabled = true;
            Animator = GetComponent<Animator>();
            FadeOut = Animator.StringToHash("FadeOut");
            _playSettingManager.PlayFadeIn = PlayFadeIn;
            _playSettingManager.PlayFadeOut = PlayFadeOut;
            DisableImage();
        }
    }
}