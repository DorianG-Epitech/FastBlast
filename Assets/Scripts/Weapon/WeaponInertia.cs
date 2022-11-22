using UnityEngine;

namespace Fastblast
{
    public class WeaponInertia : MonoBehaviour
    {
        public Transform WeaponContainer;
        public float Multiplier;
        public float Smooth;
        public float MaxYpos = 0.1f;
        public float MinYpos = 0.1f;
        private float _VelocityY = 0;
        private float _LastPositionY = 0;
        private float _basePositionY;

        private void Start()
        {
            _basePositionY = WeaponContainer.localPosition.y;
        }

        private void Update()
        {
            _VelocityY = transform.position.y - _LastPositionY;
            _LastPositionY = transform.position.y;
            Vector3 currentPosition = WeaponContainer.localPosition;
            currentPosition.y = Mathf.Lerp(currentPosition.y, _basePositionY - (_VelocityY / Multiplier), Time.deltaTime * Smooth);
            currentPosition.y = Mathf.Clamp(currentPosition.y, _basePositionY - MinYpos, _basePositionY + MaxYpos);
            WeaponContainer.localPosition = currentPosition;
        }
    }
}
