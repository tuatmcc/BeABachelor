using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BGMSourceInstaller : MonoInstaller
{
    [SerializeField] AudioSource audioSource;

    public override void InstallBindings()
    {
        Container
            .Bind<AudioSource>()
            .To<AudioSource>()
            .FromInstance(audioSource);
    }
}
