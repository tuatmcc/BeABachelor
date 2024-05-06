﻿using System;
using System.Net;
using BeABachelor.Interface;
using BeABachelor.Networking;
using BeABachelor.Networking.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace BeABachelor.PlaySetting
{
    public class ConfirmUI : PlaySettingUIBase
    {
        [SerializeField] private PlaySettingManager playSettingManager;
        
        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override void Space()
        {
            playSettingManager.ReadyStateChangeWaitFade();
        }

        public override void Back()
        {
            if(GameManager.PlayType == PlayType.Solo)
            {
                playSettingManager.State = PlaySettingState.PlayMode;
            }
        }

        public override void Activate()
        {
            gameObject.SetActive(true);
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}