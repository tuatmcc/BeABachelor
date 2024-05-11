using BeABachelor.Interface;
using BeABachelor.Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BeABachelor.PlaySetting.DI
{
    public class SettingAudioManagerInstaller : MonoInstaller
    {
        [SerializeField] private SettingAudioManager audioManager;

        public override void InstallBindings()
        {
            Container
                .Bind<IAudioManager>()
                .To<SettingAudioManager>()
                .FromInstance(audioManager)
                .AsSingle();
        }
    }
}
