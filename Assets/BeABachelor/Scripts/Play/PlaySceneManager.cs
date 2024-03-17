using BeABachelor.Play.Player;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneManager : MonoBehaviour
    {
        [Inject] GameManager _gameManager;

        [SerializeField] GameObject hakken;
        [SerializeField] GameObject kouken;

        public void Start()
        {
            // 未選択の場合
            if(_gameManager.PlayType == PlayType.NotSelected)
            {
                Debug.Log("Started Play Scene without selecting PlayType, so start with solo play");
                _gameManager.PlayType = PlayType.Solo;
                if(_gameManager.PlayerType == PlayerType.NotSelected)
                {
                    _gameManager.PlayerType = PlayerType.Hakken;
                }
            }
            // プレイタイプごとにスクリプトを選択
            switch (_gameManager.PlayType)
            {
                case PlayType.Solo:
                    if(_gameManager.PlayerType == PlayerType.Hakken)
                    {
                        kouken.SetActive(false);
                        var script = hakken.GetComponent<RemoteControlledPlayer>();
                        script.enabled = false;
                    }
                    else
                    {
                        hakken.SetActive(false);
                        var script = kouken.GetComponent<RemoteControlledPlayer>();
                        script.enabled = false;
                    }
                    break;
                case PlayType.Multi:
                    if(_gameManager.PlayerType == PlayerType.Hakken)
                    {
                        var remoteControlledPlayer = hakken.GetComponent<RemoteControlledPlayer>();
                        remoteControlledPlayer.enabled = false;
                        var keyControlledPlayer = kouken.GetComponent<KeyControlledPlayer>();
                        keyControlledPlayer.enabled = false;
                    }
                    else
                    {
                        var remoteControlledPlayer = kouken.GetComponent<RemoteControlledPlayer>();
                        remoteControlledPlayer.enabled = false;
                        var keyControlledPlayer = hakken.GetComponent<KeyControlledPlayer> ();
                        keyControlledPlayer.enabled = false;
                    }
                    break;
            }

            _gameManager.GameState = GameState.Playing;
        }
               
        public GameObject GetPlayerObject()
        {
            return _gameManager.PlayerType == PlayerType.Hakken ? hakken : kouken;
        }

    }
}
