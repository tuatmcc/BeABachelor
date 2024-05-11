using System;
using UnityEngine.Serialization;

namespace BeABachelor.Networking.Config
{
    [Serializable]
    public class NetworkConfig
    {
        public int port = 8888;
        public string[] ipAddresses = Array.Empty<string>();
        
        public override string ToString()
        {
            return $"Port: {port}, IP Addresses: {string.Join(", ", ipAddresses)}";
        }
    }
}