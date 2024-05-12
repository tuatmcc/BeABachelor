using BeABachelor.Interface;
using System;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private GameObject effect;
        [SerializeField] private GameObject deleteEffect;

        public virtual bool DestroyOnItemCollectorHit => true;

        public event Action<GameObject> OnItemCollectorHit;

        [Inject] protected IGameManager _gameManager;
        [Inject] protected IAudioManager _audioManager;
        [Inject] protected PlaySceneManager _sceneManager;
        [Inject] private ItemManager _itemManager;

        private bool used = false;

        private void OnTriggerEnter(Collider other)
        {
            if(used) return;
            if (_gameManager.GameState != GameState.Playing) return;
            if (!other.TryGetComponent(out IItemCollectable _)) return;
            ItemHit(other.gameObject);
        }

        public void ItemHit(GameObject hitObj)
        {
            if (used) return;
            used = true;
            _itemManager.ItemHitNotify(ItemID);
            OnItemCollectorHit?.Invoke(hitObj);

            if(DestroyOnItemCollectorHit)
            {
                DeleteEffect();
            }
        }

        public void DeleteEffect()
        {
            if(effect != null)
            {
                effect.SetActive(false);
            }
            if(deleteEffect != null)
            {
                deleteEffect.SetActive(true);
            }

            UniTask.Create(async () =>
            {
                await UniTask.Delay(1000);
                deleteEffect.SetActive(false);
                return UniTask.CompletedTask;
            }).Forget();
            gameObject.SetActive(false);
            _itemManager.ItemNum--;
        }
    }
}
