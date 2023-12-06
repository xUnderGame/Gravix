using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Checkpoint Behaviour")]
public class CheckpointBehaviour: ScriptableObject
{
    public Vector2 coordinates = Vector2.zero;
    public int sceneCode;
    public bool cpGravity;
    public bool forceDefaultCheckpoint;
    public string sceneChange = "DEFAULT";
    public GameObject oldCP;

    public void OnEnable() {
        forceDefaultCheckpoint = false;
        sceneChange = "DEFAULT";
        cpGravity = false;
        sceneCode = 0;
        oldCP = null;
    }

    // Gets the start position of the current scene
    public Vector2 GetSpawnpoint(string spawnpoint = "DEFAULT")
    {
        Vector2 spawnCheck;
        try { spawnCheck = GameObject.Find(spawnpoint.ToUpper()).transform.position; } // TODO FIX THIS THING
        catch {
            // I bet this creates an infinte loop somehow
            if (spawnpoint != "DEFAULT") return GetSpawnpoint();
            else return Vector2.zero;
        } 
        return spawnCheck;
    }

    // Simple check for the scene number
    public bool CheckActiveSceneNumber(int sceneNumber)
    {
        return SceneManager.GetActiveScene().buildIndex == sceneNumber;
    }
}
