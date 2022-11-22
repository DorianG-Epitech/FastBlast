using UnityEngine;

namespace Enemies
{
    public class IKFootSolver : MonoBehaviour
    {
        [SerializeField] LayerMask terrainLayer = default;
        [SerializeField] Transform legLength = default;
        [SerializeField] IKTargetStep targetStep = default;
        [SerializeField] IKFootSolver otherFoot = default;
        [SerializeField] IKFootSolver otherFoot1 = default;
        [SerializeField] IKFootSolver otherFoot2 = default;
        public float stepDistance = 4;
        public float stepHeight = 1;
        public float speed = 1;
        Vector3 oldPosition, currentPosition, newPosition;
        Vector3 oldNormal, currentNormal, newNormal;
        float lerp;
    
        private void Start()
        {
            currentPosition = newPosition = oldPosition = transform.position;
            currentNormal = newNormal = oldNormal = transform.up;
        }

        // Update is called once per frame

        void Update()
        {
            transform.position = currentPosition;

            if (Vector3.Distance(newPosition, targetStep.groundHit) > (stepDistance * transform.root.localScale.x) && !otherFoot.IsMoving() && !otherFoot1.IsMoving() && !otherFoot2.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                newPosition = new Vector3(targetStep.groundHit.x, targetStep.groundHit.y + legLength.localScale.y, targetStep.groundHit.z);
            }
            if (lerp < 1)
            {
                Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
                tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * (stepHeight * transform.root.localScale.x);

                currentPosition = tempPosition;
                lerp += Time.deltaTime * speed;
            }
            else
            {
                oldPosition = newPosition;
            }
        }
    
        public bool IsMoving()
        {
            return lerp < 1;
        }
    }
}
