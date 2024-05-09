using UnityEngine;
using Zenject;

namespace BeABachelor.Play.DI
{
    public class PlaySceneManagerInstaller : MonoInstaller
    {
        [SerializeField] private PlaySceneManager _playSceneManager;
        [SerializeField] private PlaySceneAudioManager _playSceneAudioManager;

        public override void InstallBindings()
        {
            Container
                .Bind<PlaySceneManager>()
                .To<PlaySceneManager>()
                .FromInstance(_playSceneManager)
                .AsSingle();
            Container
                .Bind<PlaySceneAudioManager>()
                .To<PlaySceneAudioManager>()
                .FromInstance(_playSceneAudioManager)
                .AsSingle();
        }
    }
}
