using BeABachelor.Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace BeABachelor.Play.Items.Effect
{
    public class ScoreItemGetEffect : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
    }
}