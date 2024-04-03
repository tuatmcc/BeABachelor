using BeABachelor;
using BeABachelor.Interface;
using BeABachelor.Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TimePresenter : MonoBehaviour
{
    [SerializeField] Text text;

    [Inject] IGameManager gameManager;
    [Inject] PlaySceneManager playSceneManager;

    void Start()
    {
        text.enabled = false;
        playSceneManager.OnTimeChanged += OnTimeChanged;
        gameManager.OnGameStateChanged += EnableText;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= EnableText;
    }

    private void OnTimeChanged(int time)
    {
        text.text = $"{time / 60 : 00}:{time % 60 : 00}";
    }

    private void EnableText(GameState gameState)
    {
        if(gameState == GameState.Playing)  text.enabled = true;
    }
}
