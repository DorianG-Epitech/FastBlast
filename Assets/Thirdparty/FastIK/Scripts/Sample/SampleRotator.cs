using UnityEngine;

namespace Thirdparty.FastIK.Scripts.Sample
{
    public class SampleRotator : MonoBehaviour
    {
        
        void Update()
        {
            //just rotate the object
            transform.Rotate(0, Time.deltaTime * 90, 0);
        }
    }
}
