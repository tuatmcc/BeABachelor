using UnityEngine;

namespace BeABachelor.Play.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private Rigidbody _rigidbody;


        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDGrounded = Animator.StringToHash("Grounded");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator.SetBool(_animIDGrounded, true);
        }

        private void Update()
        {
            _animator.SetFloat(_animIDSpeed, _rigidbody.velocity.magnitude);
            _animator.SetFloat(_animIDMotionSpeed, _rigidbody.velocity.magnitude * 0.1f);
        }
    }
}