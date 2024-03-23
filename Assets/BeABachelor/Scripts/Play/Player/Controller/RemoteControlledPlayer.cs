using System;
using UnityEngine;

namespace BeABachelor.Play.Player
{
    public class RemoteControlledPlayer : MonoBehaviour, IPlayable
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
