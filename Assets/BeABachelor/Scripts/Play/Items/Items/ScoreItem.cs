using System;
using BeABachelor.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items
{
    public class ScoreItem : ItemBase
    {
        [Inject] private IGameManager _gameManager;
        
        private void Awake()
        {
            OnItemCollectorHit += NotifyItemHit;
        }

        private void NotifyItemHit(Collider other)
        {
            // ItemIDを基にNetworkManagerに衝突を通知
            Debug.Log($"ID : {ItemID}");

            switch (_gameManager.PlayerType)
            {
                case PlayerType.Hakken when other.CompareTag("Hakken"):
                    _gameManager.Score += 2;
                    break;
                case PlayerType.Kouken when other.CompareTag("Kouken"):
                    _gameManager.OpponentScore += 2;
                    break;
                case PlayerType.NotSelected:
                    Debug.LogError("PlayerType is not selected");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
