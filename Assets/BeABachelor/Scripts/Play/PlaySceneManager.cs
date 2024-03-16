using UnityEngine;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneManager : IInitializable
    {
        [Inject] GameManager _gameManager;



        public void Initialize()
        {
            if(_gameManager.PlayType == PlayType.NotSelected)
            {
                Debug.Log("Started Play Scene without selecting PlayType, so start with solo play");
                _gameManager.PlayType = PlayType.Solo;
                if(_gameManager.PlayerType == PlayerType.NotSelected)
                {
                    _gameManager.PlayerType = PlayerType.Hakken;
                }
            }
            switch (_gameManager.PlayType)
            {
                case PlayType.Solo:
                    // プレーヤーを配置する処理
                    break;
                case PlayType.Multi:
                    // プレイヤーを配置する処理
                    // 片方はRemoteで操作される
                    break;
            }
        }


    }
}
