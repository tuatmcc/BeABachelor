using System.IO;
using System.Net;
using UnityEngine;

namespace BeABachelor.Networking
{
    public class TransformSynchronization : MonoSynchronization
    {
        private void OnConnect(EndPoint _)
        {
                if (TryGetComponent(out Rigidbody rb))
                {
                    // disable gravity
                    rb.isKinematic = UseReceivedData;
                }
        }
        private void Start()
        {
            _networkManager.OnConnected += OnConnect;
            base.Start();
        }
        
        private void OnDestroy()
        {
            _networkManager.OnConnected -= OnConnect;
        }
        
        public override byte[] ToBytes()
        {
            using var writer = new BinaryWriter(new MemoryStream(28));
            var t = transform;
            var p = t.position;
            var r = t.rotation;
            writer.Write(p.x);
            writer.Write(p.y);
            writer.Write(p.z);
            writer.Write(r.x);
            writer.Write(r.y);
            writer.Write(r.z);
            writer.Write(r.w);
            return ((MemoryStream)writer.BaseStream).ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            if(!UseReceivedData) return;
            var t = transform;
            var p = t.position;
            var r = t.rotation;
            using var reader = new BinaryReader(new MemoryStream(bytes));
            p.x = reader.ReadSingle();
            p.y = reader.ReadSingle();
            p.z = reader.ReadSingle();
            r.x = reader.ReadSingle();
            r.y = reader.ReadSingle();
            r.z = reader.ReadSingle();
            r.w = reader.ReadSingle();
            t.position = p;
            t.rotation = r;
        }
    }
}