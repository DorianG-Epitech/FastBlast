using Fastblast;
using Items;

public class FightingEntity : AliveEntity
{
    public WeaponConfiguration weaponConfiguration;

    void Awake()
    {
        Init();
    }

    public override void Init()
    {
        weaponConfiguration = Instantiate(weaponConfiguration);
        InitWeaponConfig();
        base.Init();
    }

    protected virtual void InitWeaponConfig()
    {
        weaponConfiguration.BaseAttackDamage = weaponConfiguration.AttackDamage;
        weaponConfiguration.BaseAttackRange = weaponConfiguration.AttackRange;
        weaponConfiguration.BaseAttackRate = weaponConfiguration.AttackRate;
        weaponConfiguration.BaseLifeSteal = weaponConfiguration.LifeSteal;
    }

    protected override void AddBuff(Buff newBuff)
    {
        switch (newBuff.buffType) {
            case (BuffType.ATKDMG):
                weaponConfiguration.AttackDamage = GetItemsBoosts(BuffType.ATKDMG, weaponConfiguration.BaseAttackDamage);
                break;
            case (BuffType.ATKSPEED):
                weaponConfiguration.AttackRate = GetItemsBoosts(BuffType.ATKSPEED, weaponConfiguration.BaseAttackRate);
                break;
            case (BuffType.LFSTL):
                weaponConfiguration.LifeSteal = GetItemsBoosts(BuffType.LFSTL, weaponConfiguration.BaseLifeSteal);
                break;
            case (BuffType.HLTHSTL):
                weaponConfiguration.HealthSteal = GetItemsBoosts(BuffType.HLTHSTL, weaponConfiguration.BaseHealthSteal);
                break;
            case (BuffType.BLTSIZE):
                if (weaponConfiguration.GetType() == typeof(FireArmConfiguration)) {
                    FireArmConfiguration config = weaponConfiguration as FireArmConfiguration;
                    config.BulletSize = GetItemsBoosts(BuffType.BLTSIZE, config.BaseBulletSize);
                }
                break;
            case (BuffType.BLTSPEED):
                if (weaponConfiguration.GetType() == typeof(FireArmConfiguration)) {
                    FireArmConfiguration config = weaponConfiguration as FireArmConfiguration;
                    config.BulletSize = GetItemsBoosts(BuffType.BLTSPEED, config.BaseBulletSize);
                }
                break;
            case (BuffType.BLTBNC):
                if (weaponConfiguration.GetType() == typeof(FireArmConfiguration)) {
                    FireArmConfiguration config = weaponConfiguration as FireArmConfiguration;
                    config.BulletBounces = (int)GetItemsBoosts(BuffType.BLTBNC, config.BaseBulletBounces);
                }
                break;

        }
        base.AddBuff(newBuff);
    }
}
