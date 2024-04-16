using UnityEngine;
using Zenject;

namespace BeABachelor.PlaySetting.DI
{
    public class PlaySettingManagerInstaller : MonoInstaller
    {
        [SerializeField] private PlaySettingManager playSettingManager;
        
        public override void InstallBindings()
        {
            Container.Bind<IPlaySettingManager>()
                .To<PlaySettingManager>()
                .FromInstance(playSettingManager)
                .AsSingle();
        }
    }
}