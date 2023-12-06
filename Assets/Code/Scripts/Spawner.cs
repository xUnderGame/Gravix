using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnDelay = 1.0f;
    public int spawnCap = 3;

    private Transform spawnPos;
    private List<GameObject> currentSpawns = new();

    // Start is called before the first frame update
    void Start()
    {
        // Don't start coroutine if there is no prefab
        if (!prefab) { Debug.Log($"No prefab added to {gameObject.name}!"); return; }

        // Prepare for spawning and start the coroutine
        spawnPos = gameObject.transform;
        StartCoroutine(SpawnPrefab(spawnDelay));
    }

    // Spawn prefab using a set time
    public IEnumerator SpawnPrefab(float waitfor) {
        while (true) {
            // Spawn cap + spawning the projectile
            if (currentSpawns.Count >= spawnCap) {
                currentSpawns[0].transform.position = spawnPos.position;
                currentSpawns.Add(currentSpawns[0]);
                currentSpawns.Remove(currentSpawns[0]);
            }
            else currentSpawns.Add(Instantiate(prefab, spawnPos.position, quaternion.identity, spawnPos));
            
            yield return new WaitForSeconds(waitfor);
        }
    }
}
