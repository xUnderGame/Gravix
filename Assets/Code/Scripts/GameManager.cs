using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Inspector enums for easier trigger setups.
    public enum TriggerType {ChangeGravity, KillPlayer, UpdateSpawnpoint, ChangeScene, RestrictGravity, ForceGravity, Event}
    public enum EventType {RespawnDefault, PlayerCanMove, PlayerCanSwap, PlayerCanForceRespawn, CollectCollectible}
    public enum SpawnDirections {Default, Up, Down, Left, Right}
    public enum AreaSongs {None, Area0, Area1, Area2}

    public static GameManager Instance;
    public CheckpointBehaviour spawnpoint;
    public PlayerBehaviour player;
    public GameObject playerPrefab;
    public GameObject uiPrefab;
    public Coroutine roomNameFadeCoroutine;

    // Audio clips for area songs
    public AudioClip area0Song;
    public AudioClip area1Song;
    public AudioClip area2Song;
    
    [NonSerialized] public AudioSource musicbox;
    [NonSerialized] public AreaSongs currentArea;
    [NonSerialized] public GameObject UI;
    [NonSerialized] public GameObject PauseUI;
    [NonSerialized] public GameObject roomName;
    [NonSerialized] public Text collectibleText;
    [NonSerialized] public int collectibles;
    [NonSerialized] public GameObject UInoSwap;
    [NonSerialized] public List<string> deleteOnLoad = new();
    [NonSerialized] public bool isPaused;

    void Awake()
    {
        // GameManager things
        if (!Instance) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        // No duplicate players and at least one player present
        if (!GameObject.Find("Meem")) { 
            GameObject tempPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            tempPlayer.name = "Meem";
            player.UpdatePlayerVars();
        }

        // Create UI (once)
        if (!GameObject.Find("Base UI")) {
            UI = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(UI);

            // Set UI stuff
            UI.GetComponent<Canvas>().worldCamera = Camera.main;
            UI.GetComponent<Canvas>().planeDistance = 2;
            musicbox = UI.transform.Find("Musicbox").gameObject.GetComponent<AudioSource>();
            roomName = UI.transform.Find("Room Name").gameObject;
            UInoSwap = UI.transform.Find("EFFECTS").gameObject.transform.Find("No Swap").gameObject;
            collectibleText = UI.transform.Find("ORBS").gameObject.transform.Find("Orb Count").gameObject.GetComponent<Text>();
            UInoSwap.SetActive(false);
            UI.name = "Base UI";

            // Pause menu
            PauseUI = UI.transform.Find("PAUSE").gameObject;
            PauseUI.SetActive(false);
        }

        // Scriptables Setup
        collectibles = 0;
        isPaused = false;
        spawnpoint.coordinates = spawnpoint.GetSpawnpoint();
        spawnpoint.sceneCode = SceneManager.GetActiveScene().buildIndex;
        player.MoveTo(spawnpoint.coordinates);
        player.gravity = false;
    }

    // Pauses the game and enables the pause UI
    public void GamePause() {
        EnablePauseUI(); // Toggles UI on
        PauseUI.transform.Find("DEATHS").gameObject.transform.Find("Death Count").gameObject.GetComponent<Text>().text = player.deaths.ToString(); // This is horrible, change later
        PauseUI.transform.Find("SWAPS").gameObject.transform.Find("Swaps Count").gameObject.GetComponent<Text>().text = player.swaps.ToString(); // This is horrible, change later
        musicbox.Pause(); // Pauses BGM
        Time.timeScale = 0; // Dear god
    }

    // Resumes the game and disables the pause UI
    public void GameResume(bool isMainMenu = false) {
        Time.timeScale = 1; // Brings back time
        PauseUI.SetActive(false); // Disables UI
        if (!isMainMenu && musicbox.gameObject.activeSelf) musicbox.Play(); // Resumes BGM
        isPaused = false;
    }

    // Enables the pause UI
    public void EnablePauseUI() {
        PauseUI.SetActive(true);
        isPaused = true;
    }

    // Simply using this because ScriptableObjects do not allow coroutines (i think you must use MonoBehaviour)
    public void InitiateRespawn(bool doFreezeFrames) {
        // Death "animation"
        player.waitForRespawn = true;
        StartCoroutine(FreezeFrames(doFreezeFrames));
    }

    // Disables the room name after x seconds
    public IEnumerator FadeRoomName(float time = 1.5f) {
        yield return new WaitForSecondsRealtime(time);
        roomName.SetActive(false);
    }

    // Waits some time and allows to change back the animation.
    private IEnumerator FreezeFrames(bool doFreezeFrames) {
        // Gives "freeze frames" to the death animation
        if (doFreezeFrames)
        {
            Time.timeScale = 0;
            player.renderer.color = new Color(1f, 0.6f, 0.6f);
            yield return new WaitForSecondsRealtime(0.30f);
        }
        
        // After freezing
        player.waitForRespawn = false;
        Time.timeScale = 1;

        // Change scene if spawnpoint is in another scene
        if (!spawnpoint.CheckActiveSceneNumber(spawnpoint.sceneCode)) player.ChangeScene(spawnpoint.sceneCode);

        // Respawn player normally
        player.MoveTo(spawnpoint.coordinates);
        player.ForceGravity(spawnpoint.cpGravity);
        if (doFreezeFrames) player.renderer.color = Color.white;
    }

    // Changes the current area song using the scene path
    public void ChangeAreaSong() {
        if (!musicbox.gameObject.activeSelf) return;
    
        // Logic for changing songs (not optimal)
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path.Contains("Area 0") && currentArea != AreaSongs.Area0) {
            currentArea = AreaSongs.Area0;
            musicbox.clip = area0Song;
        } else if (activeScene.path.Contains("Area 1") && currentArea != AreaSongs.Area1) {
            currentArea = AreaSongs.Area1;
            musicbox.clip = area1Song;
        } else if (activeScene.path.Contains("Area 2") && currentArea != AreaSongs.Area2) {
            currentArea = AreaSongs.Area2;
            musicbox.clip = area2Song;
        }

        // Play musicbox if it isnt playing
        if (!musicbox.isPlaying && activeScene.buildIndex != 0) musicbox.Play();
    }
}
