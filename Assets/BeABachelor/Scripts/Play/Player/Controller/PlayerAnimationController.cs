using BeABachelor.Networking;
using UnityEngine;

namespace BeABachelor.Play.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimationController : MonoBehaviour
    {
        public static readonly float AminSpeedMultiplier = 0.1f;

        [SerializeField] Animator animator;
        [SerializeField] Rigidbody rigidbody;
        [SerializeField] private PlayerAnimationSynchronization playerAnimationSynchronization;

        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDGrounded = Animator.StringToHash("Grounded");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private void Start()
        {
            animator.SetBool(_animIDGrounded, true);
        }

        private void Update()
        {
            if (playerAnimationSynchronization.UseReceivedData) return;
            animator.SetFloat(_animIDSpeed, rigidbody.velocity.magnitude);
            animator.SetFloat(_animIDMotionSpeed, rigidbody.velocity.magnitude * AminSpeedMultiplier);
        }

        private void OnFootstep() { }
    }
}