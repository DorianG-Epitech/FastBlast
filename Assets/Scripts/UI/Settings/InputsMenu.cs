using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Displays
{
    public class InputsMenu : SettingsMenu
    {
        public GameObject inputTemplate;
        public GameObject inputsParent;
        public Slider sensitivitySlider;
        public TextMeshProUGUI sensitivityValue;

        public override void SetDisplay()
        {
            base.SetDisplay();

            sensitivityValue.SetText(PlayerPrefs.GetFloat("Sensitivity", 1.0f).ToString("0.00"));
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        }

        public void SetSensitivity()
        {
            PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
            sensitivityValue.SetText(sensitivitySlider.value.ToString("0.00"));
        }
    }
}
