using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fastblast
{
    [CreateAssetMenu(fileName = "FireArmConfig", menuName = "Weapons/FireArm Config")]
    public class FireArmConfiguration : WeaponConfiguration
    {
        [Header("Bullets properties")]
        [Tooltip("The bullet shooted by the weapon.")]
        public Bullet ProjectilePrefab;
        [Tooltip("This value determines the speed of the bullets.")]
        public float BulletSpeed = 1f;
        [HideInInspector]
        public float BaseBulletSpeed = 1f;
        [Tooltip("This value determines how many enemies the bullet can hit before it disappears.")]
        public int BulletPenetration = 1;
        [HideInInspector]
        public int BaseBulletPenetration = 1;
        [Tooltip("This value determines the size of the bullet.")]
        public float BulletSize = 1f;
        [HideInInspector]
        public float BaseBulletSize = 1f;
        [Tooltip("This value determines how time the bullet can bounce on walls.")]
        public int BulletBounces = 1;
        [HideInInspector]
        public int BaseBulletBounces = 1;
    }
}
