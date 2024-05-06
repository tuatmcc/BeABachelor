using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BeABachelor.PlaySetting
{
    public class PlayModeUI : PlaySettingUIBase
    {
        [SerializeField] private PlaySettingManager playSettingManager;
        [SerializeField] private GameObject selector;
        [SerializeField] private GameObject playMode1;
        [SerializeField] private GameObject playMode2;

        private void Start()
        {
            GameManager.PlayType = PlayType.Solo;
            selector.transform.position = playMode2.transform.position;
        }

        public override void Left()
        {
            selector.transform.position = playMode1.transform.position;
            GameManager.PlayType = PlayType.Multi;
        }

        public override void Right()
        {
            selector.transform.position = playMode2.transform.position;
            GameManager.PlayType = PlayType.Solo;
        }

        public override void Space()
        {
            playSettingManager.State = GameManager.PlayType == PlayType.Solo ? PlaySettingState.Confirm : PlaySettingState.Connecting;
        }

        public override void Back()
        {
            // TODO: フェードの処理
            // GameManager.GameState = GameState.Title;
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