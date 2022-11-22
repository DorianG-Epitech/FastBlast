using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;

public class ItemsBar : MonoBehaviour
{
    public List<ItemsBarObject> itemsObjects;
    RectTransform barRect;
    public ItemsBarObject objectTemplate;

    public Vector3 objectSize = new Vector3(100, 100, 0);
    GridLayoutGroup layout;

    void Start()
    {
        layout = GetComponent<GridLayoutGroup>();
        layout.cellSize = objectSize;
        barRect = GetComponent<RectTransform>();
    }

    public void AddNewItem(EquippableItem newItem)
    {
        if (itemsObjects.Find(x => x.displayItem == newItem)) {
            itemsObjects.Find(x => x.displayItem == newItem).IncreaseAmount();
        } else {
            ItemsBarObject newObj = Instantiate(objectTemplate);
            
            newObj.InitDisplay(newItem);
            newObj.transform.SetParent(barRect.transform);
            newObj.transform.localScale = Vector3.one;
            itemsObjects.Add(newObj);
        }

        if (barRect.rect.width / (itemsObjects.Count * objectSize.x) >= 1 && barRect.rect.height / (itemsObjects.Count * objectSize.y) >= 1) {
            return;
        }
        objectSize = new Vector3(100/((barRect.transform.childCount/7)+1), 100/((barRect.transform.childCount/7)+1), 0);
        layout.cellSize = objectSize;
    }
}
