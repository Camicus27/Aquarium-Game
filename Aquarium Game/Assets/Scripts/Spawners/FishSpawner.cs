using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{

    public GameObject SpawnFish(GameObject fishPrefab)
    {
        // Spawn the fish
        GameObject fish = Instantiate(fishPrefab);

        // Spawn in a random position within boundaries
        fish.transform.position = new Vector2(
            Random.Range(-GameManager.instance.spawnBounds.x + .5f, GameManager.instance.spawnBounds.x - .5f),
            Random.Range(-GameManager.instance.spawnBounds.y + .75f, GameManager.instance.spawnBounds.y - 3.33f)
            );

        // Set a random name
        fish.GetComponent<Fish>().name = Time.time.ToString();

        return fish;
    }
}
