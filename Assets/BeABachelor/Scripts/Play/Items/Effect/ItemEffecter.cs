using System;
using UnityEngine;

namespace BeABachelor.Play.Items.Effect
{
    [RequireComponent(typeof(AudioSource))]
    public class ItemEffecter : MonoBehaviour
    {
        [SerializeField] private AudioClip effectSound;
        [SerializeField] private AudioClip getSound;
        [SerializeField] private ParticleSystem effectParticle;
        [SerializeField] private ParticleSystem getParticle;
        [SerializeField] private Animation effectAnimation;
        
        private AudioSource _audioSource;
        private int _cellected = Animator.StringToHash("Collected");

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            if (effectSound != null)
            {
                _audioSource.clip = effectSound;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (getSound != null)
                {
                    _audioSource.PlayOneShot(getSound);
                }

                if (getParticle != null)
                {
                    getParticle.Play();
                }

                if (effectAnimation != null)
                {
                    effectAnimation.Play();
                }
            }
        }
    }
}