using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Items;

public class NewItemDisplay : MonoBehaviour
{
    [SerializeField] List<EquippableItem> itemsToAdd;
    public GameObject newItemDisplay;
    public Image itemDisplayIcon;
    public TextMeshProUGUI itemDisplayName;
    public TextMeshProUGUI itemDisplayDescription;
    bool isShowingItem = false;

    Animator displayAnimator;

    public void InitDisplay()
    {
        displayAnimator = GetComponent<Animator>();
    }

    public void AddItem(EquippableItem newItem)
    {
        if (itemsToAdd == null) {
            itemsToAdd = new List<EquippableItem>();
        }
        if (itemsToAdd.Count == 0 || !itemsToAdd.Contains(newItem)) {
            itemsToAdd.Add(newItem);
        }

        if (!isShowingItem) {
            ShowItem();
            displayAnimator.Play("Open");
        }
    }

    void ShowItem()
    {
        gameObject.SetActive(true);
        isShowingItem = true;
        itemDisplayIcon.sprite = itemsToAdd[0].itemIcon;
        itemDisplayName.SetText(itemsToAdd[0].itemName);
        itemDisplayDescription.SetText(itemsToAdd[0].description);

        Invoke("HideItem", 5f);
    }

    void HideItem()
    {
        EquippableItem firstItem = itemsToAdd[0];
        itemsToAdd.RemoveAt(0);
        if (itemsToAdd.Count != 0 && itemsToAdd[0].id != firstItem.id) {
            ShowItem();
        } else {
            isShowingItem = false;
            gameObject.SetActive(false);
        }
    }
}
