using System;
using UnityEngine;

namespace BeABachelor.Networking.Test
{
    public class NetworkTestPlayer : MonoBehaviour
    {
        private void OnCollisionStay2D(Collision2D other)
        {
            NetworkTestItem item;
            Debug.Log($"CollisionStay2D: {other.gameObject.name}");
            if (other.gameObject.TryGetComponent(out item))
            {
                Debug.Log($"Item: {item.name}");
            }
        }
    }
}