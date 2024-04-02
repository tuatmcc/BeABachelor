using UnityEngine;

namespace BeABachelor.PlaySetting
{
    public class Rotator : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0, 0.5f, 0));
        }
    }
}
