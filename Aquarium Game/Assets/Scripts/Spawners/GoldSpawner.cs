using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSpawner : MonoBehaviour
{
    public Sprite silverSprite;
    public Sprite goldSprite;

    public void SpawnGold(GameObject goldPrefab, Vector3 spawnLocation, int value)
    {
        GameObject gold = Instantiate(goldPrefab) as GameObject;

        gold.transform.position = new Vector3(spawnLocation.x, spawnLocation.y, -1);
        gold.GetComponent<Gold>().value = value;

        if (value < 25)
        {
            gold.GetComponent<Gold>().sprite.sprite = silverSprite;
        }
        else if (25 <= value && value < 50)
        {
            gold.GetComponent<Gold>().sprite.sprite = goldSprite;
        }
    }
}
