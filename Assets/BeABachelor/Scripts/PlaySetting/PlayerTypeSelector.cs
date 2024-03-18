using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTypeSelector : MonoBehaviour
{
    public GameObject playerTypeTiles;
    public GameObject characters;

    public enum Tile
    {
        Hakken,
        Kouken
    };

    private bool allowInput = false;
    private Tile currentTile;

    public void AllowInput()
    {
        allowInput = true;
        GameObject current = playerTypeTiles.transform.GetChild((int)currentTile).gameObject;
        current.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void DisallowInput()
    {
        allowInput = false;
        GameObject current = playerTypeTiles.transform.GetChild((int)currentTile).gameObject;
        current.GetComponent<Renderer>().material.color = Color.green;
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject first = playerTypeTiles.transform.GetChild(0).gameObject;
        first.GetComponent<Renderer>().material.color = Color.yellow;

        GameObject hakken = characters.transform.GetChild(0).gameObject;
        hakken.transform.position = new Vector3(hakken.transform.position.x, hakken.transform.position.y, -3.0f);

        GameObject kouken = characters.transform.GetChild(1).gameObject;
        kouken.transform.position = new Vector3(kouken.transform.position.x, kouken.transform.position.y, 3.0f);

        currentTile = Tile.Hakken;
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
            GameObject current = playerTypeTiles.transform.GetChild(0).gameObject;
            current.GetComponent<Renderer>().material.color = Color.white;

            GameObject next = playerTypeTiles.transform.GetChild(1).gameObject;
            next.GetComponent<Renderer>().material.color = Color.yellow;

            GameObject hakken = characters.transform.GetChild(0).gameObject;
            hakken.transform.position = new Vector3(hakken.transform.position.x, hakken.transform.position.y, 3.0f);

            GameObject kouken = characters.transform.GetChild(1).gameObject;
            kouken.transform.position = new Vector3(kouken.transform.position.x, kouken.transform.position.y, -3.0f);

            currentTile = Tile.Kouken;
        }
        if (Input.GetKey(KeyCode.A))
        {
            GameObject current = playerTypeTiles.transform.GetChild(1).gameObject;
            current.GetComponent<Renderer>().material.color = Color.white;

            GameObject next = playerTypeTiles.transform.GetChild(0).gameObject;
            next.GetComponent<Renderer>().material.color = Color.yellow;

            GameObject hakken = characters.transform.GetChild(0).gameObject;
            hakken.transform.position = new Vector3(hakken.transform.position.x, hakken.transform.position.y, -3.0f);

            GameObject kouken = characters.transform.GetChild(1).gameObject;
            kouken.transform.position = new Vector3(kouken.transform.position.x, kouken.transform.position.y, 3.0f);

            currentTile = Tile.Hakken;
        }
    }
}
