﻿using System;
using UnityEngine.Serialization;

namespace BeABachelor.Networking.Config
{
    [Serializable]
    public class NetworkConfig
    {
        public int port = 8888;
        public string[] ipAddresses = Array.Empty<string>();
    }
}