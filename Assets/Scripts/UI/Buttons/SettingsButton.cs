using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public class SettingsButton : DisplayButton
    {
        public List<SettingsButton> otherButtons;
        Color buttonColor;
        public Color highlightedColor;
        Color defaultColor;

        public override void Setup()
        {
            base.Setup();
            buttonColor = GetComponent<Image>().color;
            defaultColor = buttonColor;
        }

        public override void OnMouseClick()
        {
            base.OnMouseClick();
            foreach (SettingsButton button in otherButtons) {
                button.display.ResetDisplay();
                button.ResetDisplay();
            }
            buttonColor = highlightedColor;
            display.SetDisplay();
        }

        public void ResetDisplay()
        {
            buttonColor = defaultColor;
        }
    }
}
