using BeABachelor.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TitleAudioManagerInstaller : MonoInstaller
{
    [SerializeField] private TitleAudioManager _titleAudioManager;

    public override void InstallBindings()
    {
        Container
            .Bind<IAudioManager>()
            .To<TitleAudioManager>()
            .FromInstance(_titleAudioManager)
            .AsSingle();
    }
}
