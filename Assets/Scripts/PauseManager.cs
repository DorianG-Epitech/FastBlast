using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Ressources.Weapon.Scripts;

public class PauseManager : MonoBehaviour
{
    public PlayerInputs _input;
    public static bool isPaused;
    float savedTimeScale;
    public InterruptingDisplay pauseDisplay;

    public BulletTime bulletTime;
    private bool isBTReleased;

    // Update is called once per frame
    void Update()
    {
        if (_input == null && GameManager.I.Player != null)
            _input = GameManager.I.Player.gameObject.GetComponent<PlayerInputs>();
        if (bulletTime == null)
        {
            GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
            foreach (GameObject w in weapons)
                bulletTime = w.GetComponent<BulletTime>();
        }
        if (_input != null)
            ManagePause();
    }

    public void SetBTRelease()
    {
        isBTReleased = true;
    }

    private void ManagePause()
    {
        if (_input.pause && _input.pausePerformed) {
            if (!pauseDisplay.gameObject.activeInHierarchy) {
                _input.pausePerformed = false;
                pauseDisplay.SetPauseDisplay();
                pauseDisplay.isPaused = true;
                savedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                AudioListener.pause = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else {
                ResetForGame();
                pauseDisplay.ResetDisplay();
            }
        }
    }

    //TODO: la fonction est utilisée qqpart?
    public void ResetTime()
    {
        Time.timeScale = savedTimeScale;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseDisplay.isPaused = false;
    }

    //Quit, Restart, Menu
    public void ResetForMenu()
    {
        _input.pausePerformed = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseDisplay.isPaused = false;
    }

    public void ResetForGame()
    {
        _input.pausePerformed = false;
        Time.timeScale = savedTimeScale;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseDisplay.isPaused = false;
        if (isBTReleased) {
            bulletTime.ReleaseBulletTime();
            isBTReleased = false;
        }

    }
}
