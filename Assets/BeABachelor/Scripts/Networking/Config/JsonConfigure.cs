using System.IO;
using UnityEngine;

namespace BeABachelor.Networking.Config
{
    public static class JsonConfigure
    {
        private static NetworkConfig _networkConfig;
        public static NetworkConfig NetworkConfig => _networkConfig ??= LoadJson<NetworkConfig>("./NetworkConfig.json");
        
        private static T LoadJson<T>(string path) where T : new()
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                var instance = new T();
                File.WriteAllText(path, JsonUtility.ToJson(instance));
                return instance;
            }
            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
    }
}