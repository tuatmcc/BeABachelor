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
        [SerializeField] private int ItemID { get; set; }

        public virtual bool DestroyOnItemCollectorHit => true;

        public event Action<Collider> OnItemCollectorHit;

        [Inject] private IGameManager _gameManager;


        private void OnTriggerEnter(Collider other)
        {
            if (_gameManager.GameState != GameState.Playing) return;
            if (other.TryGetComponent(out IItemCollectable _)) return;

            OnItemCollectorHit?.Invoke(other);

            if(DestroyOnItemCollectorHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
