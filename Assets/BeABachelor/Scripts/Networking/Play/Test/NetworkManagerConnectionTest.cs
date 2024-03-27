using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace BeABachelor.Networking.Play.Test
{
    public class NetworkManagerConnectionTest : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager1;
        
        private void Start()
        {
            networkManager1.ConnectAsync().Forget();

            UniTask.WaitUntil(() => networkManager1.IsConnected)
                .Forget();
        }
    }
}