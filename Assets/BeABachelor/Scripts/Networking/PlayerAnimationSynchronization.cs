using System.IO;
using UnityEngine;

namespace BeABachelor.Networking
{
    public class PlayerAnimationSynchronization : MonoSynchronization
    {
        [SerializeField] private Animator animator;
        
        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        
        public override byte[] ToBytes()
        {
            using BinaryWriter writer = new (new MemoryStream(8));
            writer.Write(animator.GetFloat(_animIDSpeed));
            writer.Write(animator.GetFloat(_animIDMotionSpeed));
            return ((MemoryStream)writer.BaseStream).ToArray();
        }

        public override void FromBytes(byte[] bytes)
        {
            if(!UseReceivedData) return;
            using BinaryReader reader = new (new MemoryStream(bytes));
            animator.SetFloat(_animIDSpeed, reader.ReadSingle());
            animator.SetFloat(_animIDMotionSpeed, reader.ReadSingle());
        }
    }
}