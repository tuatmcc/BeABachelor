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
            _gameManager.Score += 2;
        }
    }
}
