using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBehaviour : MonoBehaviour
{
    [SerializeField] GameManager.TriggerType triggerType;
    [SerializeField] GameManager.EventType eventType;
    [SerializeField] GameManager.SpawnDirections changeSceneSpawnDirection;
    public int sceneNumber = 0;
    public int eventRepeatBeforeDestroy = -1;
    public bool eventDestroyOnLoad = false;
    public bool triggerBool = false;

    private PlayerBehaviour player;

    public void Start() { try { player = GameManager.Instance.player; } catch { } }

    // Executes once the player enters a trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (triggerType) {

            // Change gravity
            case GameManager.TriggerType.ChangeGravity:
                player.ChangeGravity();
                break;

            // Kill player
            case GameManager.TriggerType.KillPlayer:
                GameManager.Instance.spawnpoint.forceDefaultCheckpoint = false;
                player.Respawn();
                break;

            // Update spawnpoint
            case GameManager.TriggerType.UpdateSpawnpoint:
                // Updates the spawnpoint
                GameManager.Instance.spawnpoint.coordinates = gameObject.transform.position;
                GameManager.Instance.spawnpoint.sceneCode = SceneManager.GetActiveScene().buildIndex;
                GameManager.Instance.spawnpoint.cpGravity = player.gravity;

                // Visual effect
                if (GameManager.Instance.spawnpoint.oldCP != null) {
                    if (GameManager.Instance.spawnpoint.oldCP != gameObject) GameManager.Instance.spawnpoint.oldCP.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
                }
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                GameManager.Instance.spawnpoint.oldCP = gameObject;
                break;

            // Change current scene
            case GameManager.TriggerType.ChangeScene:
                GameManager.Instance.spawnpoint.forceDefaultCheckpoint = true;
                GameManager.Instance.spawnpoint.sceneChange = changeSceneSpawnDirection.ToString();
                player.ChangeScene(sceneNumber);
                break;

            // Restrict gravity
            case GameManager.TriggerType.RestrictGravity:
                GameManager.Instance.UInoSwap.SetActive(true);
                player.restrictedGrav = true;
                break;

            // Force gravity
            case GameManager.TriggerType.ForceGravity:
                player.ForceGravity(triggerBool);
                break;

            // **EVENTS**
            case GameManager.TriggerType.Event:
                EventList();
                break;
        }
    }

    // Executes once the player leaves the trigger.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (triggerType)
        {
            // Restrict gravity
            case GameManager.TriggerType.RestrictGravity:
                GameManager.Instance.UInoSwap.SetActive(false);
                player.restrictedGrav = false;
                break;
        }
    }

    // List of events
    private void EventList(bool countDown = true) {
        switch(eventType) {
            // Respawn player at default room position
            case GameManager.EventType.RespawnDefault:
                GameManager.Instance.spawnpoint.forceDefaultCheckpoint = false;
                player.Respawn(false);
                break;

            // Toggle if the player can move 
            case GameManager.EventType.PlayerCanMove:
                player.canMove = triggerBool;
                break;

            // Toggle if the player can swap gravity
            case GameManager.EventType.PlayerCanSwap:
                player.canSwap = triggerBool;
                break;

            // Toggle if the player can (force) respawn
            case GameManager.EventType.PlayerCanForceRespawn:
                player.canForceRespawn = triggerBool;
                break;

            // Collectible +1
            case GameManager.EventType.CollectCollectible:
                GameManager.Instance.collectibles += 1;
                GameManager.Instance.collectibleText.text = GameManager.Instance.collectibles.ToString();
                break;
        }

        // Destroy the event on NEXT room load?
        if (eventDestroyOnLoad) GameManager.Instance.deleteOnLoad.Add(gameObject.name);

        // Destroy the event object after x uses?
        if (!countDown || eventRepeatBeforeDestroy == -1) return;
        eventRepeatBeforeDestroy -= 1;
        if (eventRepeatBeforeDestroy <= 0) Destroy(gameObject);
    }
}
