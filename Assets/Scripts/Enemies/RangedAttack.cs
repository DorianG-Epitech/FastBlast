using System;
using System.Collections;
using UnityEngine;
using Sound;

namespace Enemies
{
    [RequireComponent(typeof(SphereCollider))]
    public class RangedAttack : MonoBehaviour
    {
        public VFXOrientation Vfx;
        private SphereCollider Collider;
        private PlayerEntity _target;
        private Coroutine _attackCoroutine;
        private FightingEntity _attacker;

        private void Awake()
        {
            Collider = GetComponent<SphereCollider>();
            _attacker = GetComponentInParent<FightingEntity>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _target = other.GetComponent<PlayerEntity>();
            if (_target != null && _attackCoroutine == null)
                _attackCoroutine = StartCoroutine(Attack());
        }
        
        private void OnTriggerExit(Collider other)
        {
            _target = null;
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }

        private IEnumerator Attack()
        {
            while (_target != null)
            {
                if (_target != null) {
                    Vfx.PlayVFX();
                    AudioManager.PlayEnemyAttack();
                    _target.TakeDamage(_attacker.weaponConfiguration.AttackDamage);
                }
                WaitForSeconds wait = new WaitForSeconds(_attacker.weaponConfiguration.AttackRate);
                yield return wait;
            }
            _attackCoroutine = null;
        }
    }
}
