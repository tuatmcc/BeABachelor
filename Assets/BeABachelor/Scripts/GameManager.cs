using System;
using Zenject;
using BeABachelor.Interface;
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

        private GameState _gameState;
        private int _score;
        private int _tick;

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
            Debug.Log(_tick);
            if(_gameState == GameState.Playing)
            {
                _tick++;
            }
        }
    }
}
