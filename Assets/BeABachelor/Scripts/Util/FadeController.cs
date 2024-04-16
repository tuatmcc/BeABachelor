using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Util
{
    public class FadeController : MonoBehaviour
    {
        [Inject] private ITitleManager _titleManager;
        private Animator _animator;
        private int _fadeOut;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.enabled = true;
            _animator = GetComponent<Animator>();
            _fadeOut = Animator.StringToHash("FadeOut");
            _titleManager.PlayFadeIn = PlayFadeIn;
            _titleManager.PlayFadeOut = PlayFadeOut;
        }

        public void PlayFadeIn()
        {
            _animator.SetBool(_fadeOut, false);
        }

        public void PlayFadeOut()
        {
            _animator.SetBool(_fadeOut, true);
        }
    }
}
