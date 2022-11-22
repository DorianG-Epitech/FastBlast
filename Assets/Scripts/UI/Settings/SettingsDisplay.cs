using System.Collections.Generic;
using UnityEngine;

namespace UI.Displays
{
    public class SettingsDisplay : IDisplay
    {
        public List<SettingsMenu> settingsMenus;
        public List<GameObject> toHide;

        public override void SetDisplay()
        {
            base.SetDisplay();
            settingsMenus[0].SetDisplay();
            foreach (GameObject gameObject in toHide) gameObject.SetActive(false);
        }

        public override void ResetDisplay()
        {
            base.ResetDisplay();
            foreach (IDisplay display in settingsMenus) {
                if (display.gameObject.activeInHierarchy) {
                    display.ResetDisplay();
                }
            }
            foreach (GameObject gameObject in toHide) gameObject.SetActive(true);
        }
    }
}
