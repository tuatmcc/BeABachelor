using BeABachelor.Play.Items;
using System;
using UnityEngine;

namespace BeABachelor.Play.Player
{
    public class RemoteControlledPlayer : MonoBehaviour, IPlayable, IEnemyItemCollectable
    {
        public Vector3 RemoteTransform
        {
            set
            {
                transform.position = value;
            }
        }
    }
}
