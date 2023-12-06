using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{
    // Starts or resumes a game from the latest checkpoint
    public void StartGame() {
        if (!GameManager.Instance) { SceneManager.LoadScene(1); return; }
        GameManager.Instance.spawnpoint.forceDefaultCheckpoint = false;
        GameManager.Instance.player.canForceRespawn = true;
        GameManager.Instance.player.Respawn(false, false, false, true, false);
        GameManager.Instance.UI.SetActive(true);
    }

    // Quits the game
    public void ExitGame() { Application.Quit(); }

    // Restart game
    public void RestartGame() {
        Debug.LogWarning("This Isn't finished! Button does nothing.");
        if (!GameManager.Instance) { return; }

        // Removes all DeleteOnLoad stuff and resets stats.
        GameManager.Instance.deleteOnLoad = new();
        GameManager.Instance.player.deaths = 0;
        GameManager.Instance.player.swaps = 0;
        GameManager.Instance.collectibles = 0;

        // "Resets" game
        GameManager.Instance.spawnpoint.coordinates = new Vector2(-13, -2);
        GameManager.Instance.spawnpoint.sceneCode = 1;
        GameManager.Instance.spawnpoint.forceDefaultCheckpoint = false;
        GameManager.Instance.player.canForceRespawn = true;
        GameManager.Instance.UI.SetActive(true);
        GameManager.Instance.collectibleText.text = "0";
        SceneManager.LoadScene(1);
    }

    // (PAUSE UI) resumes the game if previously paused
    public void Resume(bool isMainMenu = false) { GameManager.Instance.GameResume(isMainMenu); }

    // (PAUSE UI) goes back to the main menu
    public void BackToMenu() {
        Resume(true);
        GameManager.Instance.musicbox.Pause();
        GameManager.Instance.player.ChangeScene(0);
        GameManager.Instance.spawnpoint.forceDefaultCheckpoint = true;
        GameManager.Instance.player.canForceRespawn = false;
    }

    // (PAUSE UI) Toggle player invulnerability
    public void ToggleInvulnerable(Toggle uiToggle) { if (GameManager.Instance) GameManager.Instance.player.invulnerable = uiToggle.isOn; }

    // (PAUSE UI) Toggle music player
    public void ToggleMusic(Toggle uiToggle) { if(GameManager.Instance) GameManager.Instance.musicbox.gameObject.SetActive(uiToggle.isOn); }
}
