using UnityEngine;

namespace BeABachelor.PlaySetting
{
    public class PlayerTypeUI : PlaySettingUIBase
    {
        [SerializeField] private GameObject selector;
        [SerializeField] private GameObject playerType1;
        [SerializeField] private GameObject playerType2;
        [SerializeField] private GameObject hakken;
        [SerializeField] private GameObject koken;
        public override void Left()
        {
            selector.transform.position = playerType1.transform.position;
            _gameManager.PlayerType = PlayerType.Kouken;
        }

        public override void Right()
        {
            selector.transform.position = playerType2.transform.position;
            _gameManager.PlayerType = PlayerType.Hakken;
        }
        
        public override void Space()
        {
            if (_gameManager.PlayerType == PlayerType.NotSelected)
            {
                _gameManager.PlayerType = PlayerType.Kouken;
            }
        }

        public override void Activate()
        {
            selector.transform.position = _gameManager.PlayerType == PlayerType.Hakken ? playerType2.transform.position : playerType1.transform.position;
            gameObject.SetActive(true);
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}