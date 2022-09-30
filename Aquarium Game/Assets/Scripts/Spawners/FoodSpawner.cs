using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    public List<Food> activeFoods = new List<Food>();
    public bool isSpawningFood;
    public int maxFood = 1;
    public int foodCapUpgradeCost = 200;
    public int foodQualityUpgradeCost = 300;
    private int foodSpawnCost = 5;

    public int quality = 0;
    public int hpRecovery = 10;
    public float growthStrength = 1.10f;


    private void OnMouseDown()
    {
        if (isSpawningFood && activeFoods.Count < maxFood && GameManager.instance.player.gold >= foodSpawnCost)
        {
            SpawnFood();
        }
    }

    public void SpawnFood()
    {
        GameManager.instance.player.SpendGold(foodSpawnCost);

        // Spawn the food
        GameObject food = Instantiate(foodPrefab);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        food.transform.position = new Vector3(mousePos.x, mousePos.y);
    }
}
