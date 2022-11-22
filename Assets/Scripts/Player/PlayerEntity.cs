using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using System.Linq;
using Ressources.Weapon.Scripts;
using Sound;
using Fastblast;

public class PlayerEntity : FightingEntity
{
    public WeaponAnimation weaponAnimation;
    public WeaponShooting weaponShooting;

    public float bulletTimeAmount;
    [HideInInspector]
    public float baseBTAmount;
    public float requiredBTAmount;
    [HideInInspector]
    public float baseRequiredBTAmount;

    [Header("UI")]
    public ItemsBar itemsBar;
    public NewItemDisplay newItemDisplay;
    public HealthDisplay healthDisplay;
    public InterruptingDisplay endDisplay;

    void Awake()
    {
        Init();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        baseBTAmount = bulletTimeAmount;
        baseRequiredBTAmount = requiredBTAmount;
        //TODO: fonction InitWeaponConfig dans FightingEntity qui initie les valeurs de la config
        weaponConfiguration.BaseAttackDamage = weaponConfiguration.AttackDamage;
        weaponAnimation._configuration = weaponConfiguration;
        weaponShooting._configuration = weaponConfiguration as FireArmConfiguration;

        //Setup Display

        newItemDisplay = FindObjectOfType<NewItemDisplay>();
        healthDisplay = FindObjectOfType<HealthDisplay>();
        itemsBar = FindObjectOfType<ItemsBar>();
        endDisplay = FindObjectOfType<InterruptingDisplay>();

        newItemDisplay.gameObject.SetActive(false);
        endDisplay.gameObject.SetActive(false);

        newItemDisplay.InitDisplay();
        healthDisplay.InitDisplay();
        healthDisplay.SetDisplay(maxHealth, currentHealth);
    }

    protected override void InitWeaponConfig()
    {
        FireArmConfiguration playerConfig = Instantiate(weaponConfiguration) as FireArmConfiguration;
        weaponConfiguration = playerConfig;
        base.InitWeaponConfig();
        playerConfig.BaseBulletSpeed = playerConfig.BulletSpeed;
        playerConfig.BaseBulletPenetration = playerConfig.BulletPenetration;
        playerConfig.BaseBulletSize = playerConfig.BulletSize;
        playerConfig.BaseBulletBounces = playerConfig.BulletBounces;
    }

    public override float TakeDamage(float damageAmount)
    {
        float damageDealt = base.TakeDamage(damageAmount);
        healthDisplay.SetDisplay(maxHealth, currentHealth);

        return (damageDealt);
    }

    public override void Die()
    {
        //empecher de bouger et tirer
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        endDisplay.SetEndDisplay();
    }

    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);
        healthDisplay.SetDisplay(maxHealth, currentHealth);
    }

    public void IncreaseMaxHealth(float newAmount)
    {
        maxHealth = newAmount;
        healthDisplay.SetDisplay(maxHealth, currentHealth);
    }

    public override void AddItem(EquippableItem newItem)
    {
        base.AddItem(newItem);
        itemsBar.AddNewItem(newItem);
        newItemDisplay.AddItem(newItem);
    }

    protected override void AddBuff(Buff newBuff)
    {
        switch (newBuff.buffType) {
            case (BuffType.MOREBLTTM):
                bulletTimeAmount = GetItemsBoosts(BuffType.MOREBLTTM, baseBTAmount);
                return;
            case (BuffType.HEALTH):
                maxHealth = GetItemsBoosts(BuffType.HEALTH, baseMaxHealth);
                Heal(GetItemBoost(newBuff, currentHealth));
                healthDisplay.SetDisplay(maxHealth, currentHealth);
                return;
                /*
            case (BuffType.FSTRBLTTM):
                requiredBTAmount = GetItemsBoosts(BuffType.ATKSPEED, baseAttackSpeed);
                break;
                */
        }
        base.AddBuff(newBuff);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "GroundItem") {
            other.transform.GetComponent<GroundItem>().Interact();
        }
    }
}
