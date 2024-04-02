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
        }

        public override void Right()
        {
            selector.transform.position = playerType2.transform.position;
        }
        
        public override void Space()
        {
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