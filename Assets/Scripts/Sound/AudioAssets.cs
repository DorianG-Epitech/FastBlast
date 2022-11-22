using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class AudioAssets : MonoBehaviour
    {
        private static AudioAssets _i;

        public static AudioAssets I
        {
            get
            {
                if (_i == null) _i = Instantiate(Resources.Load<AudioAssets>("Prefabs/AudioAssets"));
                return _i;
            }
        }

        public AudioClip shootingSound;
        public AudioClip jumpSound;
        public AudioClip landSound;
        public AudioClip itemPickupSound;
        //public AudioClip bulletHitSound;
        public List<AudioClip> bulletImpacts;
        public AudioClip gameMusic;
        public AudioClip menuTheme;
        public AudioClip bulletTimeStartSound;
        public AudioClip bulletTimeEndSound;
        public AudioClip enemyAttack;
        public AudioClip stepSound1;
        public AudioClip stepSound2;
        public AudioClip stepSound3;
        public AudioClip stepSound4;
        public AudioClip stepSound5;
        public AudioClip stepSound6;
        public AudioClip stepSound7;
        public AudioClip stepSound8;
        public AudioClip stepSound9;
        public AudioClip stepSound10;
    }
}
