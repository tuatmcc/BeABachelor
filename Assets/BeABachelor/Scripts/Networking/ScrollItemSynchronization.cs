using System;
using System.Collections.Generic;
using System.IO;
using BeABachelor.Play.Items;
using UnityEngine;

namespace BeABachelor.Networking
{
    public class ScrollItemSynchronization : MonoSynchronization
    {
        private List<int> _sendDeletedItemIDs;
        [SerializeField] private ItemManager _itemManager;

        public override int GetHashCode()
        {
            // ascii "item"
            return 1835365481;
        }

        private new void Start()
        {
            _sendDeletedItemIDs = new List<int>();
            _itemManager.OnItemHit += id =>
            {
                Debug.Log($"Send Item ID : {id}");
                _sendDeletedItemIDs.Add(id);
            };
            base.Start();
        }
        
        public override byte[] ToBytes()
        {
            if (_sendDeletedItemIDs.Count == 0)
            {
                return new byte[]{0x00, 0x00, 0x00, 0x00};
            }
            var ids = _sendDeletedItemIDs.ToArray();
            _sendDeletedItemIDs.Clear();
            var writer = new BinaryWriter(new MemoryStream(4 + ids.Length * 4));
            writer.Write(ids.Length);
            foreach (var id in ids)
            {
                writer.Write(id);
            }
            return ((MemoryStream)writer.BaseStream).ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var id = reader.ReadInt32();
                if(_itemManager.TryGetItemFromID(id, out var item))
                {
                    _itemManager.ItemHitNotify(id);
                    Debug.Log($"ID : {id}");
                    var scrollItem = item.GetComponent<ScoreItem>();
                    scrollItem.DeleteEffect();
                    scrollItem.GetScore2Enemy();
                }
            }
        }
    }
}