using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Items;
using TMPro;

public class ItemsBarObject : MonoBehaviour
{
    public EquippableItem displayItem;
    public Image itemIcon;
    public TextMeshProUGUI amountText;
    int amount;

    public void InitDisplay(EquippableItem newItem)
    {
        displayItem = newItem;

        itemIcon.sprite = newItem.itemIcon;
        amountText.SetText("1");
        amount = 1;
    }

    public void IncreaseAmount()
    {
        amount++;
        amountText.SetText("" + amount);
    }
}
