using Zenject;

namespace BeABachelor.Result.DI
{
    public class ResultManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IResultManager>()
                .To<ResultManager>()
                .FromNew()
                .AsSingle();
        }
    }
}