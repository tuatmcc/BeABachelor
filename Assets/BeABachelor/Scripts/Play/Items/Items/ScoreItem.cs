using UnityEngine;

namespace BeABachelor.Play.Items
{
    public class ScoreItem : ItemBase
    {
        [SerializeField] private int score = 0;

        private void Awake()
        {
            OnItemCollectorHit += AddScore;
            OnItemCollectorHit += PlaySE;
        }

        private void AddScore(Collider other)
        {
            // ItemIDを基にNetworkManagerに衝突を通知
            Debug.Log($"ID : {ItemID}");

            if(other.TryGetComponent(out IItemCollectable _))
            {
                _gameManager.Score += score;
            }
        }
        
        private void PlaySE(Collider other)
        {
            if(other.TryGetComponent(out IItemCollectable _))
            {
                ((PlaySceneAudioManager)_audioManager)?.PlayItemSE();
            }
        }

        public int GetScore()
        {
            return score;
        }
    }
}
