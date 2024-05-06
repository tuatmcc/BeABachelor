using System;
using System.Net;

namespace BeABachelor.Networking.Serializable
{
    [Serializable]
    public class NetworkConfig
    {
        public int defaultPort;
        public string[] ipAddresses;
    }
}