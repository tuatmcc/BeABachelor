using BeABachelor.Interface;
using BeABachelor.Play.Player;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using BeABachelor.Networking;
using BeABachelor.Networking.Interface;
using BeABachelor.Util;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play
{
    public class PlaySceneManager : MonoBehaviour, IFade
    {
        public event Action<int> OnCountChanged;
        public event Action<int> OnTimeChanged;

        [Inject] private IGameManager _gameManager;
        [Inject] private INetworkManager _networkManager;

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
        private bool _sceneChangeFlag;

        public void Start()
        {
            _sceneChangeFlag = false;
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
                        Destroy(script);
                    }
                    else
                    {
                        hakken.SetActive(false);
                        var script = kouken.GetComponent<RemoteControlledPlayer>();
                        Destroy(script);
                    }
                    break;
                case PlayType.Multi:
                    if(_gameManager.PlayerType == PlayerType.Hakken)
                    {
                        var remoteControlledPlayer = hakken.GetComponent<RemoteControlledPlayer>();
                        Destroy(remoteControlledPlayer);
                        var keyControlledPlayer = kouken.GetComponent<KeyControlledPlayer>();
                        Destroy(keyControlledPlayer);
                        hakken.GetComponent<TransformSynchronization>().UseReceivedData = false;
                        hakken.GetComponent<PlayerAnimationSynchronization>().UseReceivedData = false;
                        kouken.GetComponent<TransformSynchronization>().UseReceivedData = true;
                        kouken.GetComponent<PlayerAnimationSynchronization>().UseReceivedData = true;
                    }
                    else
                    {
                        var remoteControlledPlayer = kouken.GetComponent<RemoteControlledPlayer>();
                        Destroy(remoteControlledPlayer);
                        var keyControlledPlayer = hakken.GetComponent<KeyControlledPlayer> ();
                        Destroy(keyControlledPlayer);
                        hakken.GetComponent<TransformSynchronization>().UseReceivedData = true;
                        hakken.GetComponent<PlayerAnimationSynchronization>().UseReceivedData = true;
                        kouken.GetComponent<TransformSynchronization>().UseReceivedData = false;
                        kouken.GetComponent<PlayerAnimationSynchronization>().UseReceivedData = false;
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
                // 相手まち
                while (!_networkManager.OpponentReady && !token.IsCancellationRequested)
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

            FinishPlay();
        }

        public GameObject GetPlayerObject()
        {
            return _gameManager.PlayerType == PlayerType.Hakken ? hakken : kouken;
        }

        public async UniTask FinishPlay()
        {
            _cts?.Cancel();
            if(_gameManager.GameState == GameState.Playing && !_sceneChangeFlag)
            {
                _sceneChangeFlag = true;
                _gameManager.GameState = GameState.Finished;
                await UniTask.Delay(3000);
                PlayFadeOut?.Invoke();
                await UniTask.Delay(1500);
                _gameManager.GameState = GameState.Result;
            }
            else if(_gameManager.GameState == GameState.Playing && _sceneChangeFlag)
            {
                Debug.LogError("Scene change flag is already set, so cannot change scene");
            }
        }

        public KeyControlledPlayer GetKeyControlledPlayer()
        {
            return _gameManager.PlayerType == PlayerType.Hakken ? 
                hakken.GetComponent<KeyControlledPlayer>() : kouken.GetComponent<KeyControlledPlayer>();
        }

        public Action PlayFadeIn { get; set; }
        public Action PlayFadeOut { get; set; }
    }
}
