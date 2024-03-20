using UnityEngine;

namespace BeABachelor.Networking
{
    public abstract record InteractionObject
    {
        public bool ReRequestFlag;
        public int TickCount;
    }
}