using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Camera
{
    public class CurrentCameraRegister : MonoBehaviour
    {
        [Inject] private PlaySceneManager _playSceneManager;
        
        private void Awake()
        {
            _playSceneManager.MainCamera = GetComponent<UnityEngine.Camera>();
        }
    }
}