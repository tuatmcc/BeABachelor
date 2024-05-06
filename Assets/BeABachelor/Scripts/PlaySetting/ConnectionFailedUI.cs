using UnityEngine;

namespace BeABachelor.PlaySetting
{
    public class ConnectionFailedUI : PlaySettingUIBase
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
            playSettingManager.State = PlaySettingState.PlayMode;
        }

        public override void Back()
        {
            playSettingManager.State = PlaySettingState.PlayMode;
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