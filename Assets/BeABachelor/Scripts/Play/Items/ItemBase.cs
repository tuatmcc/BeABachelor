using BeABachelor.Interface;
using System;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items
{
    /// <summary>
    /// アイテム
    /// </summary>
    public abstract class ItemBase : MonoBehaviour
    {
        [SerializeField] public int ItemID { get; set; }

        public virtual bool DestroyOnItemCollectorHit => true;

        public event Action<Collider> OnItemCollectorHit;

        [Inject] protected IGameManager _gameManager;
        [Inject] private ItemManager _itemManager;

        private bool used = false;

        private void OnTriggerEnter(Collider other)
        {
            if(used) return;
            if (_gameManager.GameState != GameState.Playing) return;
            if (!(other.TryGetComponent(out IItemCollectable _) || other.TryGetComponent(out IEnemyItemCollectable _))) return;

            OnItemCollectorHit?.Invoke(other);
            used = true;

            if(DestroyOnItemCollectorHit)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _itemManager.ItemNum--;
        }
    }
}
