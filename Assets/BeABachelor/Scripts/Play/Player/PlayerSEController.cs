using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeABachelor.Play.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]

    public class PlayerSEController : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] Animator animator;
        [SerializeField] AudioClip walkSE;
        [SerializeField] AudioClip runSE;

        private enum playerState
        {
            Idle,
            Walk,
            Run,
        }

        private readonly int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        private readonly float sep = DefaultPlaySceneParams.DefaultSpeed * PlayerAnimationController.AminSpeedMultiplier;
        private playerState _preState = playerState.Idle;

        void Start()
        {
            audioSource.loop = true;
        }

        void FixedUpdate()
        {
            var speed = animator.GetFloat(_animIDMotionSpeed);
            var now_state = GetPlayerState(speed);
            if(now_state != _preState)
            {
                audioSource.Stop();
                switch (now_state)
                {
                    case playerState.Idle:
                        break;
                    case playerState.Walk:
                        audioSource.clip = walkSE;
                        audioSource.Play();
                        break;
                    case playerState.Run:
                        audioSource.clip = runSE;
                        audioSource.Play();
                        break;
                }
            }
            _preState = now_state;
        }
        
        private playerState GetPlayerState(float speed)
        {
            if (speed <  sep / 2)
            {
                return playerState.Idle;
            } 
            else if (sep / 2 <= speed && speed <= sep + 0.1f)
            {
                return playerState.Walk;
            }
            else
            {
                return playerState.Run;
            }
        }
    }
}
