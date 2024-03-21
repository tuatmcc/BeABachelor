using BeABachelor.Play.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムに関する情報を保持
/// </summary>
public class ItemManager : MonoBehaviour
{
    [SerializeField] List<GameObject> items;

    private void Awake()
    {
        // 各アイテムにIDを割り当て(1～)
        var count =  1;
        foreach (GameObject item in items)
        {
            var itembase = item.GetComponent<ItemBase>();
            itembase.ItemID = count;
            count++;
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
