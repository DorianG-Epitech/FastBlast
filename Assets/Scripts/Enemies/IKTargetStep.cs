using UnityEngine;

namespace Enemies
{
    public class IKTargetStep : MonoBehaviour
    {

        public LayerMask layerMask;
        public Vector3 groundHit;
        
        void Update() {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 500f, layerMask)) {
                Vector3 targetLocation = hit.point;
                targetLocation += new Vector3(0, transform.localScale.y + 0.1f, 0);
                groundHit = targetLocation;
            }
        }
    }
}