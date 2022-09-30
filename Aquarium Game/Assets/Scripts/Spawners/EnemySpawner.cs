using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPre;
    public TextMeshProUGUI enemyCountdown;
    public GameObject enemyWarning;
    public bool canSpawn = true;
    public int spawnRate = 125;
    public double timeSinceLastEnemySpawn = 120;

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
    private IEnumerator SpawnEnemyCountdown()
    {
        // Setup countdown
        enemyCountdown.text = "5.00";
        enemyWarning.SetActive(true);
        double countdown = 5.00;

        while (countdown > 0.02)
        {
            countdown -= .01;

            enemyCountdown.text = countdown.ToString("0.##");

            yield return new WaitForSecondsRealtime(.01f);
        }

        // Spawn the enemy
        enemyWarning.SetActive(false);
        GameManager.instance.SpawnEnemy(enemyPre);
    }

    private void FixedUpdate()
    {
        if (Time.timeAsDouble > timeSinceLastEnemySpawn)
        {
            timeSinceLastEnemySpawn = Time.timeAsDouble + spawnRate + Random.Range(-5f, 25f);
            StartCoroutine(SpawnEnemyCountdown());
        }
    }
}
