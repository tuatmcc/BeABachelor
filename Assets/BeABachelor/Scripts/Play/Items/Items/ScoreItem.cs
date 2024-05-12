using UnityEngine;

namespace BeABachelor.Play.Items
{
    public class ScoreItem : ItemBase
    {
        [SerializeField] private int score = 0;

        private void Awake()
        {
            OnItemCollectorHit += NotifyItemHit;
            OnItemCollectorHit += PlaySE;
        }

        private void NotifyItemHit(GameObject other)
        {
            // ItemIDを基にNetworkManagerに衝突を通知
            Debug.Log($"ID : {ItemID}");

            if(other.TryGetComponent(out IItemCollectable _))
            {
                GetScore();
            }
        }
        
        public void GetScore()
        {
            _gameManager.Score += score;
        }
        
        public void GetScore2Enemy()
        {
            _gameManager.EnemyScore += score;
        }

        private void PlaySE(GameObject other)
        {
            if(other.TryGetComponent(out IItemCollectable _))
            {
                ((PlaySceneAudioManager)_audioManager)?.PlayItemSE();
            }
        }
    }
}
