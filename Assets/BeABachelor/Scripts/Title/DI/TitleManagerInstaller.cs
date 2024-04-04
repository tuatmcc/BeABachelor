using UnityEngine;
using Zenject;

namespace BeABachelor.Title.DI
{
    public class TitleManagerInstaller : MonoInstaller
    {
        [SerializeField] private TitleManager _titleManager;
        
        public override void InstallBindings()
        {
            Container.Bind<ITitleManager>()
                .To<TitleManager>()
                .FromInstance(_titleManager)
                .AsSingle();
        }
    }
}