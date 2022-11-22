using Sound;
using UnityEngine;
using UnityEngine.VFX;

namespace Fastblast
{
    [RequireComponent(typeof(WeaponConfiguration))]
    public class WeaponShooting : MonoBehaviour
    {
        public GameObject DebugBullet;

        public Transform PlayerCamera;
        public Transform BulletsOrigin;
        public LayerMask RaycastableLayers;
        public VisualEffect ShootingEffect;
        [HideInInspector]
        public FireArmConfiguration _configuration;

        public void OnShootEvent()
        {
            if (GameManager.I.Player.GetCurrentHealth() <= 0) {
                return ;
            }
            RaycastHit hit;
            Vector3 hitPoint = PlayerCamera.position + (PlayerCamera.forward * 50f);
        
            if (Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out hit, 50f, RaycastableLayers))
            {
                hitPoint = hit.point;
            }
            Bullet bullet = Instantiate(_configuration.ProjectilePrefab, BulletsOrigin.position, Quaternion.Euler(Vector3.zero));
            bullet.InitBulletProperties(_configuration);
            Vector3 bulletVelocity = ((hitPoint - BulletsOrigin.position)).normalized * _configuration.BulletSpeed;
            bullet.LaunchBullet(bulletVelocity);
            ShootingEffect.SendEvent("OnPlay");
            AudioManager.PlayShootingSound();
        }

        private void Awake()
        {
            //_configuration = GameManager.I.Player.weaponConfiguration as FireArmConfiguration;
        }

        private void OnDrawGizmos()
        {
            RaycastHit hit;
            float hitDistance = 50f;
            Vector3 hitPoint = PlayerCamera.position + (PlayerCamera.forward * 50f);

            if (Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out hit, 50f, RaycastableLayers)) 
            {
                hitPoint = hit.point;
                hitDistance = hit.distance;
                DebugBullet = hit.collider.gameObject;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(BulletsOrigin.position, BulletsOrigin.position + ((hitPoint - BulletsOrigin.position)).normalized * hitDistance);    
        }
    }
}
