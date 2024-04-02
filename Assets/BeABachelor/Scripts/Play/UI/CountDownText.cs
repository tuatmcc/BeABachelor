using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeABachelor.Play.UI
{
    public class CountDownText : MonoBehaviour
    {
        [SerializeField] private Text text;
        
        public void CountText(int count)
        {
            gameObject.SetActive(true);
            text.text = count.ToString();
            if (count == 0)
            {
                GoText().Forget();
            }
        }
        
        private async UniTask GoText()
        {
            text.text = "GO!";
            await UniTask.Delay(2000);
            gameObject.SetActive(false);
        }
    }
}