using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Sound
{
    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem I;

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
        }

        public AudioSource sfxSource;
        public AudioSource musicSource;
        public AudioMixer audioMixer;
        
        public void Start()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
                AudioManager.PlayMenuTheme();
            else
                AudioManager.PlayGameTheme();
        }

        public void Update()
        {
        }
    
    }
}