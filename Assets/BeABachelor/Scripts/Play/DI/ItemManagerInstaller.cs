using BeABachelor.Play.Items;
using UnityEngine;
using Zenject;

namespace BeABachelor.DI
{
        public class ItemManagerInstaller : MonoInstaller
        {
            [SerializeField] private ItemManager _itemManager;
            public override void InstallBindings()
            {
                Container
                    .Bind<ItemManager>()
                    .To<ItemManager>()
                    .FromInstance(_itemManager)
                    .AsSingle();
            }

        }
}
