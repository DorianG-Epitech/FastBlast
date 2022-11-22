using UnityEngine;
using UnityEngine.InputSystem;

namespace Fastblast
{
    [RequireComponent(typeof(WeaponConfiguration), typeof(Animator))]
    public class WeaponAnimation : MonoBehaviour
    {
        [HideInInspector]
        public WeaponConfiguration _configuration;
        private Animator _animator;

        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (GameManager.I.Player.GetCurrentHealth() <= 0) {
                return ;
            }
            bool IsShooting = context.ReadValueAsButton();
            _animator.SetBool("Shooting", IsShooting);
        }

        private void Awake()
        {
            //_configuration = GameManager.I.Player.weaponConfiguration;
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (_animator.GetBool("Shooting"))
            {
                if (_animator.speed != _configuration.AttackRate)
                    _animator.speed = _configuration.AttackRate;
            }
            else
            {
                if (_animator.speed != 1f)
                    _animator.speed = 1f;
            }
        }
    }
}
