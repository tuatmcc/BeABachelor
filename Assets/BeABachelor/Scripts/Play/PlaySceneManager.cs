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
        public event Action<int> OnTimeChanged;

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
                Debug.Log($"Count changed : {_count}");
            }
        }

        public int MainTimer
        {
            get => _timer;
            set
            {
                _timer = value;
                OnTimeChanged?.Invoke(_timer);
                // Debug.Log($"Timer : {MainTimer}");
            }
        }

        private int _count;
        private int _timer;

        private CancellationTokenSource _cts;
        private bool _counting;
        private long _startTime;

        public void Start()
        {
            // 未選択の場合
            if(_gameManager.PlayType == PlayType.NotSelected)
            {
                Debug.Log("Started Play Scene without selecting PlayType, so start with Solo play");
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

            Count = DefaultPlaySceneParams.CountLengthSceond;
            MainTimer = DefaultPlaySceneParams.PlayLengthSecond;

            _gameManager.OnGameStateChanged += OnGameStateChanged;

            _cts = new();
            WaitConnectionAsync( _cts.Token ).Forget();
        }

        private void Update()
        {
            if (_counting)
            {
                
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _gameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.CountDown:
                    _cts = new();
                    StartCountdownAsync( _cts.Token ).Forget();
                    break;
                case GameState.Playing:
                    _cts = new();
                    StartPlayAsync(_cts.Token).Forget();
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
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken : token);
                Count--;
            }
            _gameManager.GameState = GameState.Playing;
        }

        private async UniTask StartPlayAsync(CancellationToken token)
        {
            while(MainTimer > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                MainTimer--;
            }

            _gameManager.GameState = GameState.Finished;
        }

        public GameObject GetPlayerObject()
        {
            return _gameManager.PlayerType == PlayerType.Hakken ? hakken : kouken;
        }

    }
}
