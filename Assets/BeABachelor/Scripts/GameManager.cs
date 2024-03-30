using System;
using Zenject;
using BeABachelor.Interface;
using UnityEngine.SceneManagement;

namespace BeABachelor
{
    /// <summary>
    /// ゲームの状態を管理するクラス
    /// </summary>
    public class GameManager : IGameManager, IInitializable, IDisposable
    {
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnScoreChanged;

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

        public void NextScene()
        {
            switch (_gameState)
            {
                case GameState.Title:
                    SceneManager.LoadScene("PlaySetting");
                    GameState = GameState.Setting;
                    break;
                case GameState.Setting:
                    SceneManager.LoadScene("Play");
                    GameState = GameState.Ready;
                    break;
                case GameState.Finished:
                    SceneManager.LoadScene("Result");
                    GameState = GameState.Result;
                    break;
                case GameState.Result:
                    SceneManager.LoadScene("Title");
                    GameState = GameState.Title;
                    break;
                case GameState.Ready:
                case GameState.CountDown:
                case GameState.Playing:
                default:
                    throw new InvalidOperationException("無効なタイミングでのシーン遷移です。");
            }
        }

        public void ToTitle()
        {
            SceneManager.LoadScene("Title");
            Reset();
        }

        private GameState _gameState;
        private int _score;

        public void Initialize()
        {
            Reset();
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
        }
    }
}
