using System;
using System.Threading;
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
            UniTask.WaitUntil(() => networkManager1.IsConnected).Forget();
            
            // Hoge().Forget();
            // Fuga().Forget();
        }

        private async UniTask Hoge()
        {
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(5));
            var cancellationTokenSource = new CancellationTokenSource();
            var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationTokenSource.Token, timeoutToken).Token;
            var task = Observable.Interval(TimeSpan.FromSeconds(0.5f), cancellationToken: token)
                .Subscribe(_ => Debug.Log("Hoge"));
            await UniTask.WaitUntil(() =>
            {
                Debug.Log("Fuga");
                return false;
            }, cancellationToken: timeoutToken);
            Debug.Log("Fuga2");
            task.Dispose();
            if (timeoutToken.IsCancellationRequested)
            {
                Debug.Log("Timeout");
                return;
            }
            else
            {
                Debug.Log("Success");
            }
            //timeController.Reset();
            timeController.Reset();
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            if (timeoutToken.IsCancellationRequested)
            {
                Debug.Log("Timeout");
            }
            else
            {
                Debug.Log("Success");
            }
        }

        private async UniTask Fuga()
        {
            var timeController = new TimeoutController();
            var timeoutToken = timeController.Timeout(TimeSpan.FromSeconds(3));
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || timeoutToken.IsCancellationRequested);
            Debug.Log("こんにちは");
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || timeoutToken.IsCancellationRequested);
            Debug.Log("お元気ですか");
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || timeoutToken.IsCancellationRequested);
            Debug.Log("さようなら");
        }
    }
}