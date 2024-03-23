using UnityEngine;

namespace BeABachelor.Play.Items
{
    public class ScoreItem : ItemBase
    {
        private void Awake()
        {
            OnItemCollectorHit += NotifyItemHit;
        }

        private void NotifyItemHit(Collider other)
        {
            // ItemIDを基にNetworkManagerに衝突を通知
            Debug.Log($"ID : {ItemID}");
        }
    }
}
