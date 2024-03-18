using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeSelector : MonoBehaviour
{
    public GameObject playModeTiles;

    public enum Tile
    {
        SinglePlayer,
        MultiPlayer
    };

    private bool allowInput = false;
    private Tile currentTile;

    public void AllowInput()
    {
        allowInput = true;
        GameObject current = playModeTiles.transform.GetChild((int)currentTile).gameObject;
        current.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void DisallowInput()
    {
        allowInput = false;
        GameObject current = playModeTiles.transform.GetChild((int)currentTile).gameObject;
        current.GetComponent<Renderer>().material.color = Color.green;
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject first = playModeTiles.transform.GetChild(0).gameObject;
        first.GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowInput)
        {
            return;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Debug.Log("D key is pressed");
            GameObject current = playModeTiles.transform.GetChild(0).gameObject;
            current.GetComponent<Renderer>().material.color = Color.white;
            GameObject next = playModeTiles.transform.GetChild(1).gameObject;
            next.GetComponent<Renderer>().material.color = Color.yellow;
            currentTile = Tile.MultiPlayer;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A key is pressed");
            GameObject current = playModeTiles.transform.GetChild(1).gameObject;
            current.GetComponent<Renderer>().material.color = Color.white;
            GameObject next = playModeTiles.transform.GetChild(0).gameObject;
            next.GetComponent<Renderer>().material.color = Color.yellow;
            currentTile = Tile.SinglePlayer;
        }
    }
}
