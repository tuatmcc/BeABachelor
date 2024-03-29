using System.IO;
using UnityEngine;

namespace BeABachelor.Networking
{
    public class TransformSynchronization : MonoSynchronization
    {

        private void Start()
        {
            _networkManager.OnConnected += _ =>
            {
                if (_networkManager.IsHost == !Match2Host && TryGetComponent(out Rigidbody rb))
                {
                    // disable gravity
                    rb.useGravity = false;
                }
            };
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
            if(Match2Host != _networkManager.IsHost) return;
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