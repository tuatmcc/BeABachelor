using BeABachelor.Interface;
using UnityEngine;
using Zenject;

namespace BeABachelor.Result
{
    public class ResultCharacterToggler : MonoBehaviour
    {
        [SerializeField] private GameObject green;
        [SerializeField] private GameObject blue;

        [Inject] private IGameManager _gameManager;

        private void Start()
        {
            if (_gameManager.PlayerType == PlayerType.Hakken)
            {
                green.SetActive(true);
                blue.SetActive(false);
            }
            else
            {
                green.SetActive(false);
                blue.SetActive(true);
            }
        }
    }
}