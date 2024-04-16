using Zenject;

namespace BeABachelor.Result.DI
{
    public class ResultManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind(typeof(IResultManager), typeof(IInitializable))
                .To<ResultManager>()
                .FromNew()
                .AsSingle();
        }
    }
}