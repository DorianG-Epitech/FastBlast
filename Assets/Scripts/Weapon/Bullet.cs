using Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Fastblast;
using Enemies;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public List<string> EntityTagList;
    public VisualEffect ElectricityCollision;
    private Rigidbody _rigidbody;
    [SerializeField]
    private FireArmConfiguration _configuration;
    private Vector3 _initialeVelocity;
    private int _remainingBounces;
    private float _remainingLife = 30;

    public void InitBulletProperties(FireArmConfiguration configuration)
    {
        _configuration = configuration;
        transform.localScale = (Vector3.one * 0.3f) * _configuration.BulletSize;
        _remainingBounces = _configuration.BulletBounces;
        
    }

    public void LaunchBullet(Vector3 bulletVelocity)
    {
        _rigidbody.velocity = bulletVelocity;
        _initialeVelocity = bulletVelocity;
    }

    public float GetBulletDamages()
    {
        return _configuration.AttackDamage;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _remainingLife -= Time.deltaTime;
        if (_remainingLife < 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        ContactPoint contactPoint  = other.GetContact(0);
        ElectricityCollision.transform.up = contactPoint.normal;
        ElectricityCollision.SetVector3("ContactNormal", contactPoint.normal);
        ElectricityCollision.SetVector3("ContactAngle", Quaternion.LookRotation(contactPoint.normal, Vector3.down).eulerAngles);
        ElectricityCollision.SetVector3("ContactPosition", transform.InverseTransformPoint(contactPoint.point));
        ElectricityCollision.SendEvent("OnBulletCollision");

        if (EntityTagList.Contains(other.gameObject.tag))
        {
            EnemyEntity hitEnemy = other.gameObject.GetComponent<EnemyEntity>();
            float damageTaken = hitEnemy.TakeDamage(GetBulletDamages());
            if (_configuration.LifeSteal > 0) {
                float lifeSteal = damageTaken * (_configuration.LifeSteal/100);
                GameManager.I.Player.Heal(lifeSteal);
            }

            if (hitEnemy.GetCurrentHealth() <= 0 && _configuration.HealthSteal > 0) {
                GameManager.I.Player.IncreaseMaxHealth(GameManager.I.Player.maxHealth + _configuration.HealthSteal);
                GameManager.I.Player.Heal(_configuration.HealthSteal);
            }
            AudioManager.PlayBulletHitSound();
            Destroy(gameObject);
        } 
        else if (_remainingBounces == 0)
        {
            ElectricityCollision.transform.SetParent(null);
            Destroy(ElectricityCollision.gameObject, 3);
            Destroy(gameObject);
        }
        else
        {
            _rigidbody.velocity = Vector3.Reflect(_initialeVelocity, contactPoint.normal);
            _initialeVelocity = _rigidbody.velocity;
            _remainingBounces--;
        }
    }
}
