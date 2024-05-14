using System;
using System.Collections.Generic;
using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items
{
    /// <summary>
    /// アイテムに関する情報を保持
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Inject] private PlaySceneManager playSceneManager;
        [Inject] private IGameManager gameManager;
        
        public Action<int> OnItemHit;
    
        private List<GameObject> items = new List<GameObject>();

        public int ItemNum
        {
            get => _itemnum;
            set
            {
                _itemnum = value;
                Debug.Log($"ItemNum changed: {value}");
                if(_itemnum == 0)
                {
                    playSceneManager.FinishPlay().Forget();
                }
            }
        }

        private int _itemnum;

        private void Awake()
        {
            // 各アイテムにIDを割り当て(1～)
            var count =  transform.childCount;
            ItemNum = count;
            for(int i = 0; i < count; i++)
            {
                var item = transform.GetChild(i).GetChild(0);
                var itembase = item.GetComponent<ItemBase>();
                itembase.ItemID = i;
                items.Add(item.gameObject);
            }
        }

        private void OnDisable()
        {
            OnItemHit = null;
        }

        public void ItemHitNotify(int itemID)
        {
            OnItemHit?.Invoke(itemID);
        }

        public bool EnemyGetItem(int ID)
        {
            if(TryGetItemFromID(ID, out var item))
            {
                Debug.Log($"Enemy got item. ID : {ID}");
                var scoreItem = item.GetComponent<ScoreItem>();
                if (scoreItem.Used) return false;
                scoreItem.Used = true;
                gameManager.EnemyScore += scoreItem.GetScore();
                scoreItem.DeleteEffect();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryGetItemFromID(int ID, out GameObject found)
        {
            foreach (GameObject item in items)
            {
                var itembase = item.GetComponent<ItemBase>();
                if(itembase.ItemID == ID)
                {
                    found = item;
                    return true;
                }
            }
            found = null;
            return false;
        }
    }
}
