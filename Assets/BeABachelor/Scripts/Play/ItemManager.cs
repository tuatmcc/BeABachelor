using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items
{
    /// <summary>
    /// アイテムに関する情報を保持
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Inject] PlaySceneManager playSceneManager;
    
        private List<GameObject> items = new List<GameObject>();

        public int ItemNum
        {
            get => _itemnum;
            set
            {
                _itemnum = value;
                if(_itemnum == 0)
                {
                    playSceneManager.FinishPlay();
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
                var item = transform.GetChild(i);
                var itembase = item.GetComponent<ItemBase>();
                itembase.ItemID = i;
                items.Add(item.gameObject);
            }
        }

        // IDからGameObjectを削除
        // 相手からのデータを処理する際の呼び出しを想定
        public void DestroyItem(int ID)
        {
            if (TryGetItemFromID(ID, out var item)) Destroy(item);
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
