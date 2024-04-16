﻿using System;
using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultManager : IResultManager
    {
        [Inject] private IGameManager _gameManager;

        public string ResultText => _gameManager.ResultState switch
        {
            ResultState.Win => "卒業成功！",
            ResultState.Lose => "留年...",
            ResultState.Draw => "仲良く卒業！",
            _ => throw new NotImplementedException()
        };

        public string ScoreText => $"{_gameManager.Score}単位取得！";

        public void ToTile()
        {
            StateChangeWithFade().Forget();
        }

        private async UniTask StateChangeWithFade()
        {
            await UniTask.Delay(1000);
            PlayFadeOut?.Invoke();
            await UniTask.Delay(1500);
            _gameManager.GameState = GameState.Title;
        }

        public Action PlayFadeIn { get; set; }
        public Action PlayFadeOut { get; set; }
    }
}