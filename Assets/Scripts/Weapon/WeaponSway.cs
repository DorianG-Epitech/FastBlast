using UnityEngine;

namespace Fastblast
{
    public class WeaponSway : MonoBehaviour
    {
        public float Smooth;
        public float Multiplier;

        private void Update()
        {
            if (GameManager.I.Player.GetCurrentHealth() <= 0) {
                return ;
            }
            float mouseX = Input.GetAxisRaw("Mouse X") * Multiplier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Multiplier;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Smooth * Time.deltaTime);
        }
    }
}
