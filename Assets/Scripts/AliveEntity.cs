using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

public class AliveEntity : Entity
{
    [Header("Health")]
    protected float currentHealth;
    [HideInInspector]
    public float baseMaxHealth;
    public float maxHealth;
    public float damageReduction;
    public float baseDamageReduction;

    [Header("Movement")]
    public float moveSpeed;
    [HideInInspector]
    public float baseMoveSpeed;
    [HideInInspector]
    public int baseNbJumps;
    public int nbJumps;

    public List<EquippableItem> equippedItems;

    private bool isALive = true;

    public override void Init()
    {
        base.Init();
        currentHealth = maxHealth;
        equippedItems = new List<EquippableItem>();
        baseMaxHealth = maxHealth;
        baseDamageReduction = damageReduction;
        baseMoveSpeed = moveSpeed;
        baseNbJumps = nbJumps;
    }

    public virtual float TakeDamage(float damageAmount)
    {
        float damagesToTake = damageAmount;
        
        if (damageReduction != 0) {
            damagesToTake -= (damagesToTake * (damageReduction/100));
        }
        currentHealth -= damagesToTake;
        if (currentHealth <= 0) {
            currentHealth = 0;

            if (isALive)
            {
                isALive = false;
                Invoke("Die", 0.1f);
            }
        }
        return (damagesToTake);
    }

    public float GetCurrentHealth()
    {
        return (currentHealth);
    }

    public virtual void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void AddItem(EquippableItem newItem)
    {
        equippedItems.Add(newItem);
        foreach (Buff buff in newItem.itemBuffs) {
            AddBuff(buff);
        }
    }

    protected virtual void AddBuff(Buff newBuff)
    {
        switch (newBuff.buffType) {
            case (BuffType.HEALTH):
                //TODO: est ce qu'on augmente un peu la vie actuelle du joueur aussi?
                maxHealth = GetItemsBoosts(BuffType.HEALTH, baseMaxHealth);
                break;
            case (BuffType.SPEED):
                moveSpeed = GetItemsBoosts(BuffType.SPEED, baseMoveSpeed);
                break;
            case (BuffType.JUMPNB):
                nbJumps = (int)GetItemsBoosts(BuffType.JUMPNB, baseNbJumps);
                break;
            case (BuffType.DMGRDC):
                damageReduction = GetItemsBoosts(BuffType.DMGRDC, baseDamageReduction);
                if (damageReduction > 90) {
                    damageReduction = 90;
                }
                break;
        }
    }

    protected float GetItemsBoosts(BuffType buffType, float baseValue)
    {
        //TODO: return une nouvelle valeur, ca devrait pas renvoyer base value
        List<Buff> buffs = new List<Buff>();
        List<Buff> percentageBuffs;
        List<Buff> solidBuffs;
        float boost = 0f;

        foreach (EquippableItem item in equippedItems) {
            foreach (Buff buff in item.itemBuffs) {
                if (buff.buffType == buffType) {
                    buffs.Add(buff);
                }
            }
        }
        percentageBuffs = new List<Buff>(buffs.FindAll(x => x.amountType == Buff.AmountType.PERCENTAGE));
        solidBuffs = new List<Buff>(buffs.FindAll(x => x.amountType == Buff.AmountType.SOLID));
        foreach (Buff solidBuff in solidBuffs) {
            baseValue += solidBuff.amount;
        }
        foreach (Buff percentageBuff in percentageBuffs) {
            boost += percentageBuff.amount;
        }
        //TODO: peut etre meilleure manière de gérer si la valeur boostée est égale à 0
        if (baseValue == 0 && boost != 0) {
            baseValue = 1;
        }
        baseValue += (baseValue * boost/100);
        return (baseValue);
    }

    protected float GetItemBoost(Buff newBuff, float baseValue)
    {
        float newValue = 0;

        switch (newBuff.amountType) {
            case(Buff.AmountType.PERCENTAGE):
                newValue += newBuff.amount;
                break;
            case(Buff.AmountType.SOLID):
                newValue += (baseValue * newBuff.amount/100);
                break;
        }
        return (newValue);
    }
}
