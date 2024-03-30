using System;

namespace BeABachelor.Interface
{
    public interface IGameManager
    {
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnScoreChanged;

        public GameState GameState { get; set; }
        public PlayType PlayType { get; set; }
        public PlayerType PlayerType { get; set; }
        public int Score { get; set; }
        public void NextScene();
        public void ToTitle();
    }
}
