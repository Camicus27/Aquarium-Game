using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ButtonType { Fish, Enemy, FoodCap, FoodQuality, Key };

public class ButtonSetup : MonoBehaviour
{
    public ButtonType buttonType;

    public GameObject fishPrefab;
    public TextMeshProUGUI fishPrice;
    public GameObject enemyPrefab;
    public TextMeshProUGUI foodCap;
    public TextMeshProUGUI foodCapIncrPrice;
    public TextMeshProUGUI foodQuality;
    public TextMeshProUGUI foodQualityIncrPrice;
    

    private void Start()
    {
        // Setup the button onClick to a corresponding callback
        switch (buttonType)
        {
            case ButtonType.Fish:
                // Set initial price display text
                fishPrice.text = "$" + fishPrefab.GetComponent<Fish>().cost;

                // Set onClick callback
                gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                    GameManager.instance.SpawnFish(fishPrefab);
                });
                break;

            case ButtonType.Enemy:
                // Set onClick callback
                gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                    GameManager.instance.SpawnEnemy(enemyPrefab);
                });
                break;

            case ButtonType.FoodCap:
                // Set initial price display text
                foodCapIncrPrice.text = "$" + GameManager.instance.foodSpawner.foodCapUpgradeCost;

                // Set onClick callback
                gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                    GameManager.instance.IncrementFoodSpawnCap();
                    foodCap.text = "Max: " + GameManager.instance.foodSpawner.maxFood;
                    foodCapIncrPrice.text = "$" + GameManager.instance.foodSpawner.foodCapUpgradeCost;
                });
                break;

            case ButtonType.FoodQuality:
                // Set initial quality/price display text
                foodQuality.text = "Quality: " + GameManager.instance.foodSpawner.quality;
                foodQualityIncrPrice.text = "$" + GameManager.instance.foodSpawner.foodQualityUpgradeCost;

                // Set onClick callback
                gameObject.GetComponent<Button>().onClick.AddListener(delegate {
                    GameManager.instance.IncrementFoodQuality();
                    foodQuality.text = "Quality: " + GameManager.instance.foodSpawner.quality;
                    foodQualityIncrPrice.text = "$" + GameManager.instance.foodSpawner.foodQualityUpgradeCost;
                });
                break;
        }


        /*if (!isEnemyButton)
        {
            // Setup the button onClick to spawn the corresponding fishPrefab (assigned in inspector)
            gameObject.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.SpawnFish(fishPrefab); });
        }
        else
        {
            // Setup the button onClick to spawn the corresponding enemyPrefab (assigned in inspector)
            gameObject.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.SpawnEnemy(enemyPrefab); });
        }*/
    }
}
