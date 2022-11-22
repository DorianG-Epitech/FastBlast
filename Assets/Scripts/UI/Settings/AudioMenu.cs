using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Displays
{
    public class AudioMenu : SettingsMenu
    {
        // public Slider masterSlider;
        // public TextMeshProUGUI masterValue;
        public Slider soundsSlider;
        public TextMeshProUGUI soundsValue;
        public Slider musicSlider;
        public TextMeshProUGUI musicValue;

        public override void SetDisplay()
        {
            base.SetDisplay();
            // masterSlider.minValue = 0.0001f;
            // masterSlider.maxValue = 1;
            // masterSlider.value = masterSlider.maxValue;
            
            soundsSlider.minValue = 0.0001f;
            soundsSlider.maxValue = 1;
            soundsSlider.value = soundsSlider.maxValue;
        
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1;
            musicSlider.value = musicSlider.maxValue;
        }

        // public void SetMasterLevel()
        // {
        //     masterValue.SetText("" + (int)masterSlider.value);
        //     AudioSystem.I.audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
        // }
        
        public void SetSoundLevel()
        {
            soundsValue.SetText("" + (int)soundsSlider.value);
            Debug.Log(Mathf.Log10(soundsSlider.value) * 20);
            AudioSystem.I.audioMixer.SetFloat("SFXVolume", Mathf.Log10(soundsSlider.value) * 20);
        }

        public void SetMusicLevel()
        {
            musicValue.SetText("" + (int)musicSlider.value);
            AudioSystem.I.audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicSlider.value) * 20);

        }
    }
}
