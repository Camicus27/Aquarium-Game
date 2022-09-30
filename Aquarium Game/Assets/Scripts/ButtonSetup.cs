using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour
{
    public GameObject fishPrefab;
    public GameObject enemyPrefab;
    public bool isEnemyButton;

    private void Start()
    {
        if (!isEnemyButton)
        {
            // Setup the button onClick to spawn the corresponding fishPrefab (assigned in inspector)
            gameObject.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.SpawnFish(fishPrefab); });
        }
        else
        {
            // Setup the button onClick to spawn the corresponding enemyPrefab (assigned in inspector)
            gameObject.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.SpawnEnemy(enemyPrefab); });
        }
    }
}
