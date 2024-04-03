using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeABachelor.Result.DI
{
    public class ResultSceneUIManager : MonoBehaviour
    {
        [SerializeField] private Text resultText;
        [SerializeField] private Text scoreText;
        
        [Inject] private IResultManager _resultManager;
        
        
    }
}