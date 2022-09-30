using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    public bool isSpawningFood;

    private void OnMouseDown()
    {
        if (isSpawningFood && GameManager.instance.activeFoods.Count < GameManager.instance.maxFood && GameManager.instance.player.gold >= 5)
        {
            SpawnFood();
        }
            
    }

    public void SpawnFood()
    {
        GameManager.instance.player.SpendGold(5);

        // Spawn the food
        GameObject food = Instantiate(foodPrefab);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        food.transform.position = new Vector3(mousePos.x, mousePos.y);
    }
}
