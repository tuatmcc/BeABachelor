using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Zenject;

public class TitleManager : MonoBehaviour
{
    // [Inject] private GameManager gameManager;

    // Start is called before the first frame update
    void Start() {} // Nothing to do

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // gameManager++;
            Debug.Log("Imprement scene transition to GameScene.");
        }
    }
}
