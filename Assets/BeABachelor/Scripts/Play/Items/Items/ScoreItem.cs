using UnityEngine;

namespace BeABachelor.Play.Items
{
    public class ScoreItem : ItemBase
    {
        [SerializeField] private int score = 0;

        private void Awake()
        {
            OnItemCollectorHit += NotifyItemHit;
        }

        private void NotifyItemHit(Collider other)
        {
            // ItemIDを基にNetworkManagerに衝突を通知
            Debug.Log($"ID : {ItemID}");

            if(other.TryGetComponent(out IItemCollectable _))
            {
                _gameManager.Score += score;
            }
            else if(other.TryGetComponent(out IEnemyItemCollectable _))
            {
                _gameManager.EnemyScore += score;
            }
        }
    }
}
