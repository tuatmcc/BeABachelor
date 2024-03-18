using BeABachelor.Play.Player;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneManager : MonoBehaviour
    {
        public event Action<int> OnCountChanged;

        [Inject] GameManager _gameManager;

        [SerializeField] GameObject hakken;
        [SerializeField] GameObject kouken;

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnCountChanged?.Invoke(_count);
                Debug.Log($"Count changed {_count}");
            }
        }

        private int _count;

        private CancellationTokenSource _cts;

        public void Start()
        {
            // 未選択の場合
            if(_gameManager.PlayType == PlayType.NotSelected)
            {
                Debug.Log("Started Play Scene without selecting PlayType, so start with solo play");
                _gameManager.PlayType = PlayType.Solo;
                if(_gameManager.PlayerType == PlayerType.NotSelected)
                {
                    _gameManager.PlayerType = PlayerType.Kouken;
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

            Count = DefaultPlaySceneParams.CountLength;

            _cts = new CancellationTokenSource();
            WaitConnectionAsync( _cts.Token ).Forget();

            _gameManager.GameState = GameState.Playing;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    _cts = new CancellationTokenSource();
                    
                    break;
            }
        }

        private async UniTask WaitConnectionAsync(CancellationToken token)
        {
            if(_gameManager.PlayType == PlayType.Multi)
            {
                while (!_gameManager.Connected || !token.IsCancellationRequested)
                {
                    await UniTask.Delay(100);
                    Debug.Log("Wait for connection established");
                }
            }
            _gameManager.GameState = GameState.CountDown;
        }

        private async UniTask StartCountdownAsync(CancellationToken token)
        {
            while(Count > 0)
            {
                await UniTask.Delay(1000);
                Count--;
            }
        }

        public GameObject GetPlayerObject()
        {
            return _gameManager.PlayerType == PlayerType.Hakken ? hakken : kouken;
        }

    }
}
