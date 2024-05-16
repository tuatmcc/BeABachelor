using System.IO;
using System.Net;
using UnityEngine;

namespace BeABachelor.Networking
{
    public class TransformSynchronization : MonoSynchronization
    {
        private Rigidbody _rb;
        
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
            _rb = GetComponent<Rigidbody>();
            _networkManager.OnConnected += OnConnect;
            base.Start();
        }
        
        private void OnDestroy()
        {
            _networkManager.OnConnected -= OnConnect;
        }
        
        public override byte[] ToBytes()
        {
            using var writer = new BinaryWriter(new MemoryStream(52));
            var t = transform;
            var p = t.position;
            var r = t.rotation;
            var vec = _rb.velocity;
            var ang = _rb.angularVelocity;
            writer.Write(p.x);
            writer.Write(p.y);
            writer.Write(p.z);
            writer.Write(r.x);
            writer.Write(r.y);
            writer.Write(r.z);
            writer.Write(r.w);
            writer.Write(vec.x);
            writer.Write(vec.y);
            writer.Write(vec.z);
            writer.Write(ang.x);
            writer.Write(ang.y);
            writer.Write(ang.z);
            return ((MemoryStream)writer.BaseStream).ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            if(!UseReceivedData) return;
            Vector3 p;
            Quaternion r;
            Vector3 vec;
            Vector3 ang;
            using var reader = new BinaryReader(new MemoryStream(bytes));
            p.x = reader.ReadSingle();
            p.y = reader.ReadSingle();
            p.z = reader.ReadSingle();
            r.x = reader.ReadSingle();
            r.y = reader.ReadSingle();
            r.z = reader.ReadSingle();
            r.w = reader.ReadSingle();
            vec.x = reader.ReadSingle();
            vec.y = reader.ReadSingle();
            vec.z = reader.ReadSingle();
            ang.x = reader.ReadSingle();
            ang.y = reader.ReadSingle();
            ang.z = reader.ReadSingle();
            transform.position = p;
            transform.rotation = r;
            _rb.velocity = vec;
            _rb.angularVelocity = ang;
        }
    }
}