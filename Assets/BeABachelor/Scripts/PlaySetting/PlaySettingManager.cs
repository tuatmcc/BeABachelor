using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Zenject;

public class PlaySettingManager : MonoBehaviour
{
    // [inject] private GameManager gameManager;
    public GameObject playModeTile;
    public GameObject playerTypeTile;

    // Start is called before the first frame update
    void Start()
    {
        playModeTile.GetComponent<PlayModeSelector>().AllowInput();
        playerTypeTile.GetComponent<PlayerTypeSelector>().DisallowInput();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // gameManager++;
            Debug.Log("PlayMode: " + playModeTile.GetComponent<PlayModeSelector>().GetCurrentTile());
            Debug.Log("PlayerType: " + playerTypeTile.GetComponent<PlayerTypeSelector>().GetCurrentTile());
        }

        if (Input.GetKey(KeyCode.S))
        {
            playModeTile.GetComponent<PlayModeSelector>().DisallowInput();
            playerTypeTile.GetComponent<PlayerTypeSelector>().AllowInput();
        }
        if (Input.GetKey(KeyCode.W))
        {
            playModeTile.GetComponent<PlayModeSelector>().AllowInput();
            playerTypeTile.GetComponent<PlayerTypeSelector>().DisallowInput();
        }
    }
}
