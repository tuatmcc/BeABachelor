using Zenject;

namespace BeABachelor.Play.DI
{
    public class PlaySceneManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<PlaySceneManager>()
                .FromNew()
                .AsSingle();
        }
    }
}
