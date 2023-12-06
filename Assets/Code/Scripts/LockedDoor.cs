using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public int numberOfCollectibles = 0;

    // Executes once the player enters the radius of the door.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance.collectibles >= numberOfCollectibles) Destroy(gameObject.transform.parent.gameObject);
    }
}
