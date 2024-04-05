using System.IO;
using UnityEngine;

namespace BeABachelor.Networking
{
    public class PlayerAnimationSynchronization : MonoSynchronization
    {
        private Animator _animator;
        private Rigidbody _rigidbody;
        
        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        
        public override byte[] ToBytes()
        {
            using var writer = new BinaryWriter(new MemoryStream(8));
            writer.Write(_animator.GetFloat(_animIDSpeed));
            writer.Write(_animator.GetFloat(_animIDMotionSpeed));
            return ((MemoryStream)writer.BaseStream).ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            if(!UseReceivedData) return;
            using var reader = new BinaryReader(new MemoryStream(bytes));
            _animator.SetFloat(_animIDSpeed, reader.ReadSingle());
            _animator.SetFloat(_animIDMotionSpeed, reader.ReadSingle());
        }
    }
}