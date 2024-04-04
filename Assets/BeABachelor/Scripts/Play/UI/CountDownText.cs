using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace BeABachelor.Play.UI
{
    public class CountDownText : MonoBehaviour
    {
        [SerializeField] private Text text;

        private CancellationTokenSource _cts;

        private void Start()
        {
            _cts = new CancellationTokenSource();
        }

        public void CountText(int count)
        {
            gameObject.SetActive(true);
            text.text = count.ToString();
            if (count == 0)
            {
                GoText(_cts.Token).Forget();
            }
        }
        
        private async UniTask GoText(CancellationToken token)
        {
            text.text = "GO!";
            await UniTask.Delay(2000, cancellationToken:token);
            gameObject.SetActive(false);
        }
    }
}