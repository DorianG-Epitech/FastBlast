using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fastblast;
using UnityEngine.SceneManagement;

public class InterruptingDisplay : IDisplay
{
    [Header("Interruption")]
    public TextMeshProUGUI interruptionTitle;
    public TextMeshProUGUI interruptionDescription;
    
    [Header("Pause")]
    public PauseManager pauseManager;
    public bool isPaused = false;
    public List<Button> pauseButtons;
    public List<string> pauseStatus;

    [Header("End")]
    public string victoryText;
    public string defeatText;
    public List<Button> endButtons;

    public override void SetDisplay()
    {
        gameObject.SetActive(true);
        base.SetDisplay();
    }

    public override void ResetDisplay()
    {
        base.ResetDisplay();
        foreach (Button button in pauseButtons) {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in endButtons) {
            button.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void SetPauseDisplay()
    {
        SetDisplay();
        interruptionTitle.SetText("Pause");
        interruptionDescription.SetText(pauseStatus[Random.Range(0, pauseStatus.Count)]);
        foreach (Button button in pauseButtons) {
            button.gameObject.SetActive(true);
        }
    }

    public void SetEndDisplay()
    {
        SetDisplay();
        interruptionTitle.SetText("Game Over");
        if (GameManager.I.Player.GetCurrentHealth() <= 0) {
            interruptionDescription.SetText(defeatText);
        } else {
            interruptionDescription.SetText(victoryText);
        }
        foreach (Button button in endButtons) {
            button.gameObject.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        pauseManager.ResetForGame();
        ResetDisplay();
    }

    public void GoToMenu()
    {
        pauseManager.ResetForMenu();
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        pauseManager.ResetForMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        pauseManager.ResetForMenu();
        Application.Quit();
        ResetDisplay();
    }
}
