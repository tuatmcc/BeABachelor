using BeABachelor;
using BeABachelor.Play;
using UnityEngine;
using Zenject;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [Inject] GameManager _gameManager;
    [Inject] PlaySceneManager _playSceneManager;

    private void Awake()
    {
        _gameManager.OnGameStateChanged += SetTarget;
    }

    void Start()
    {
    }

    void Update()
    {
        transform.position = target.transform.position - target.transform.forward * 10 + target.transform.up * 5;
    }

    private void OnDestroy()
    {
        _gameManager.OnGameStateChanged -= SetTarget;
    }

    private void SetTarget(GameState gameState)
    {
        if(gameState == GameState.Playing)
        {
            target = _playSceneManager.GetPlayerObject();
        }
    }
}
