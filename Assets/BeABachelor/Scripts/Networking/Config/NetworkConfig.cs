using System;
using UnityEngine.Serialization;

namespace BeABachelor.Networking.Config
{
    [Serializable]
    public class NetworkConfig
    {
        public int packetSendRate = 50;
        public int port = 8888;
    }
}