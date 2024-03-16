using System;
using Zenject;

namespace BeABachelor
{
    /// <summary>
    /// ゲームの状態を管理するクラス
    /// </summary>
    public class GameManager : IInitializable, IDisposable
    {
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnScoreChanged;

        public GameState GameState 
        { 
            get => GameState;
            set 
            {
                GameState = value;
                OnGameStateChanged?.Invoke(GameState);
            }
        }

        public PlayerType PlayerType { get; set; }
        public PlayType PlayType { get; set; }

        public int Score
        {
            get => Score;
            set
            {
                Score = value;
                OnScoreChanged?.Invoke(Score);
            }
        }

        private GameState _gameState;

        public void Initialize()
        {
            Reset();
        }

        public void Dispose()
        {

        }

        public void Reset()
        {
            GameState = GameState.Title;
            PlayerType = PlayerType.NotSelected;
            PlayType = PlayType.NotSelected;
            Score = 0;
        }
    }
}
