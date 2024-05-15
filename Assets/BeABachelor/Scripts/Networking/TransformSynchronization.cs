using BeABachelor.Interface;
using System.IO;
using System.Net;
using UnityEngine;
using Zenject;

namespace BeABachelor.Networking
{
    public class TransformSynchronization : MonoSynchronization
    {
        private Vector3 p = new Vector3();
        private Quaternion r = Quaternion.identity;

        private void OnConnect(EndPoint _)
        {
                if (TryGetComponent(out Rigidbody rb))
                {
                    // disable gravity
                    rb.isKinematic = UseReceivedData;
                }
        }

        public override int GetHashCode()
        {
            return (gameObject.name + "trans").GetHashCode();
        }

        private new void Start()
        {
            _networkManager.OnConnected += OnConnect;
            p = transform.position;
            r = transform.rotation;
            base.Start();
        }

        private void Update()
        {
            if (!UseReceivedData) return;
            transform.position = p;
            transform.rotation = r;
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
            using var reader = new BinaryReader(new MemoryStream(bytes));
            p.x = reader.ReadSingle();
            p.y = reader.ReadSingle();
            p.z = reader.ReadSingle();
            r.x = reader.ReadSingle();
            r.y = reader.ReadSingle();
            r.z = reader.ReadSingle();
            r.w = reader.ReadSingle();
        }
    }
}