using System;
using Zenject;
using BeABachelor.Interface;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace BeABachelor
{
    /// <summary>
    /// ゲームの状態を管理するクラス
    /// </summary>
    public class GameManager : IGameManager, IInitializable, IDisposable, IFixedTickable
    {
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnOpponentScoreChanged;

        public bool Connected { get; set; }

        public GameState GameState 
        { 
            get => _gameState;
            set 
            {
                _gameState = value;
                OnGameStateChanged?.Invoke(_gameState);
            }
        }

        public PlayerType PlayerType { get; set; }
        public PlayType PlayType { get; set; }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnScoreChanged?.Invoke(_score);
            }
        }

        public int EnemyScore {  get; set; }
        public ResultState ResultState { get; set; }

        private GameState _gameState;
        private int _score;
        private int _tick;

        public void Initialize()
        {
            Reset();
            OnGameStateChanged += state =>
            {
                switch (state)
                {
                    case GameState.Title:
                        SceneManager.LoadScene("Title");
                        break;
                    case GameState.Setting:
                        SceneManager.LoadScene("PlaySetting");
                        break;
                    case GameState.Ready:
                        SceneManager.LoadScene("Play");
                        break;
                    case GameState.Result:
                        ResultState = _score > EnemyScore ? ResultState.Win : _score < EnemyScore ? ResultState.Lose : ResultState.Draw;
                        SceneManager.LoadScene("Result");
                        break;
                    case GameState.CountDown:
                        break;
                    case GameState.Playing:
                        break;
                    case GameState.Finished:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            };
        }

        public void Dispose()
        {

        }

        public void Reset()
        {
            Connected = false;
            GameState = GameState.Title;
            PlayerType = PlayerType.NotSelected;
            PlayType = PlayType.NotSelected;
            Score = 0;
            EnemyScore = 0;
            OnGameStateChanged += state =>
            {
                if (state == GameState.Playing)
                {
                    _tick = 0;
                }
            };
        }

        public void FixedTick()
        {
            if(_gameState == GameState.Playing)
            {
                _tick++;
            }
        }
    }
}
