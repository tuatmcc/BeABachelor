using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
