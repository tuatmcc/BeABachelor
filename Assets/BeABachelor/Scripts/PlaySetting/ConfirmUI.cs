﻿using Cysharp.Threading.Tasks;

namespace BeABachelor.PlaySetting
{
    public class ConfirmUI : PlaySettingUIBase
    {
        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override void Space()
        {
            _playSettingManager.StateChangeWaitFade().Forget();
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