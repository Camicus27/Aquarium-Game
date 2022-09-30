using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject enemy = Instantiate(enemyPrefab);

        float firstRandom = Random.value;
        float x;
        // Left side of screen
        if (firstRandom > .5)
            x = Random.Range(-GameManager.instance.spawnBounds.x + .4f, -GameManager.instance.spawnBounds.x + .75f);
        // Right side of screen
        else
            x = Random.Range(GameManager.instance.spawnBounds.x - .75f, GameManager.instance.spawnBounds.x - .4f);

        // Spawn in a random position within boundaries
        enemy.transform.position = new Vector2(x, Random.Range(-GameManager.instance.spawnBounds.y + .75f, 0));

        return enemy;
    }
}
