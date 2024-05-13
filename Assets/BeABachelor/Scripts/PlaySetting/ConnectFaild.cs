using Zenject;

namespace BeABachelor.PlaySetting
{
    public class ConnectFailed : PlaySettingUIBase
    {
        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override void Space()
        {
            _playSettingManager.State = PlaySettingState.PlayMode;
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