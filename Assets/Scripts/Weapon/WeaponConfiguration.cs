using UnityEngine;

namespace Fastblast
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/Weapon Config")]
    public class WeaponConfiguration : ScriptableObject
    {
        [Header("Weapon properties")]
        [Tooltip("This value determines the firing speed of the weapon.")]
        public float AttackRate = 0.5f;
        [HideInInspector]
        public float BaseAttackRate;
        [Tooltip("This value determines the range at which the weapon can be used")]
        public float AttackRange = 3f;
        [HideInInspector]
        public float BaseAttackRange;
        [Tooltip("This value determines the damages given to targets.")]
        public float AttackDamage = 10f;
        [HideInInspector]
        public float BaseAttackDamage;
        [Tooltip("This value determines the health earned when attacking an enemy")]
        public float LifeSteal = 0f;
        [HideInInspector]
        public float BaseLifeSteal;
        [Tooltip("This value determines the health increase when killing an enemy")]
        public float HealthSteal = 0f;
        [HideInInspector]
        public float BaseHealthSteal;
    }
}
