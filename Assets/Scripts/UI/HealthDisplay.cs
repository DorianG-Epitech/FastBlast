using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    Slider healthSlider;
    TextMeshProUGUI healthText;

    public void InitDisplay()
    {
        healthSlider = GetComponentInChildren<Slider>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetDisplay(float maxHealth, float currentHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthText.SetText((int)currentHealth + "/" + (int)maxHealth);
    }
}
