using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BeABachelor.PlaySetting
{
    public class PlayModeUI : PlaySettingUIBase
    {
        [SerializeField] private GameObject selector;
        [SerializeField] private GameObject playMode1;
        [SerializeField] private GameObject playMode2;

        private void Start()
        {
            _gameManager.PlayType = PlayType.Solo;
            selector.transform.position = playMode2.transform.position;
        }

        public override void Left()
        {
            selector.transform.position = playMode1.transform.position;
            _gameManager.PlayType = PlayType.Multi;
        }

        public override void Right()
        {
            selector.transform.position = playMode2.transform.position;
            _gameManager.PlayType = PlayType.Solo;
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