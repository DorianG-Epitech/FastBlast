using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public static class AudioManager
    {
        public static void PlayShootingSound()
        {
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.shootingSound, 0.2f);
        }
        
        public static void PlayJumpSound()
        {
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.jumpSound);
        }
        
        public static void PlayLandingSound()
        {
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.landSound);
        }
        
        public static void PlayPickupSound()
        {
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.itemPickupSound);
        }
        
        public static void PlayBulletHitSound()
        {
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.bulletImpacts[Random.Range(0, AudioAssets.I.bulletImpacts.Count)], 0.2f);
        }
        
        public static void PlayBulletTimeStartSound()
        {
            // AudioSystem.I.audioSource.pitch = 2;
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.bulletTimeStartSound, 0.1f);
        }
        
        public static void PlayBulletTimeEndSound()
        {
            // AudioSystem.I.audioSource.pitch = 2;
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.bulletTimeEndSound, 0.1f);
        }

        public static void PlayEnemyAttack()
        {
            AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.enemyAttack, 0.2f);
        }
        
        public static void PlayGameTheme()
        {
            AudioSystem.I.musicSource.loop = true;
            AudioSystem.I.musicSource.PlayOneShot(AudioAssets.I.gameMusic, 0.5f);
        }
        
        public static void PlayMenuTheme()
        {
            AudioSystem.I.musicSource.loop = true;
            AudioSystem.I.musicSource.PlayOneShot(AudioAssets.I.menuTheme, 0.5f);
        }
        
        public static void PlayStepSound()
        {
            int rng = Random.Range(0, 10);
            if (rng == 0)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound1, 0.3f);
            else if (rng == 1)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound2, 0.3f);
            else if (rng == 2)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound3, 0.3f);
            else if (rng == 3)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound4, 0.3f);
            else if (rng == 4)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound5, 0.3f);
            else if (rng == 5)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound6, 0.3f);
            else if (rng == 6)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound7, 0.3f);
            else if (rng == 7)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound8, 0.3f);
            else if (rng == 8)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound9, 0.3f);
            else if (rng == 9)
                AudioSystem.I.sfxSource.PlayOneShot(AudioAssets.I.stepSound10, 0.3f);
        }
    }
}