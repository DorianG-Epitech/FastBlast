using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Displays
{
    public class VideoMenu : SettingsMenu
    {
        public Image fullscreenIcon;
        public Dropdown resolutionDropdown;
        Resolution[] resolutions;

        public override void SetDisplay()
        {
            base.SetDisplay();
            fullscreenIcon.gameObject.SetActive(Screen.fullScreen);
            resolutions = Screen.resolutions;
            Debug.Log(resolutions.Length);
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolution = 0;
            for (int i = 0; i < resolutions.Length; i++) {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                if (!options.Contains(option)) {
                    options.Add(option);
                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) {
                        currentResolution = i;
                    }
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolution;
            resolutionDropdown.RefreshShownValue();
        }

        public void ChangeFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
            fullscreenIcon.gameObject.SetActive(!fullscreenIcon.gameObject.activeInHierarchy);
        }

        public void ChangeResolution()
        {
            Resolution resolution = resolutions[resolutionDropdown.value];
            Debug.Log($"width: {resolution.width}, height: {resolution.height}");

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
