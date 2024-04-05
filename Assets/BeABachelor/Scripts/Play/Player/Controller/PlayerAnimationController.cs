using UnityEngine;

namespace BeABachelor.Play.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] Rigidbody rigidbody;


        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDGrounded = Animator.StringToHash("Grounded");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private void Start()
        {
            animator.SetBool(_animIDGrounded, true);
        }

        private void Update()
        {
            animator.SetFloat(_animIDSpeed, rigidbody.velocity.magnitude);
            animator.SetFloat(_animIDMotionSpeed, rigidbody.velocity.magnitude * 0.1f);
        }
    }
}