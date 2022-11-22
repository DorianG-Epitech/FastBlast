using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    
    [System.Serializable]
    public struct ItemsPool {
        public EquippableItem.Rarity rarity;
        public List<GroundItem> rarityItems;
    }
    public List<ItemsPool> itemsPools;

    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public GroundItem GetRandomItem(EquippableItem.Rarity itemRarity)
    {
        ItemsPool pool = itemsPools.Find(x => x.rarity == itemRarity);
        return (pool.rarityItems[Random.Range(0, pool.rarityItems.Count)]);
    }
}
