using UnityEngine;
using Zenject;

namespace BeABachelor.Play.DI
{
    public class PlaySceneManagerInstaller : MonoInstaller
    {
        [SerializeField] private PlaySceneManager _playSceneManager;
        public override void InstallBindings()
        {
            Container
                .Bind<PlaySceneManager>()
                .To<PlaySceneManager>()
                .FromInstance(_playSceneManager)
                .AsSingle();
        }
    }
}
