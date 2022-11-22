using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public enum BuffType { HEALTH, SPEED, ATKSPEED, ATKDMG, BLTSIZE, BLTSPEED, BLTBNC, JUMPNB, MOREBLTTM, FSTRBLTTM, LFSTL, HLTHSTL, DMGRDC };
    [System.Serializable]
    public struct Buff {
        public BuffType buffType;
        public enum AmountType { SOLID, PERCENTAGE };
        public AmountType amountType;
        public int amount;
    };

    [CreateAssetMenu(fileName = "EquippableItem", menuName = "Items/Equippable Item")]
    public class EquippableItem : Item
    {
        public enum Rarity { COMMON, UNCOMMON, RARE };
        public Rarity itemRarity;
        public Sprite itemIcon;

        [SerializeField]
        public List<Buff> itemBuffs;
    }
}