using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // References
    [HideInInspector] public Player player;
    public GameObject eventManager;
    public GameObject playerObj;
    public GameObject audioObj;
    private AudioManager audioManager;
    public GameObject adManagerObj;
    private AdsManager adManager;
    public GoldSpawner goldSpawner;
    public FishSpawner fishSpawner;
    public FoodSpawner foodSpawner;
    public EnemySpawner enemySpawner;
    public GameObject goldPrefab;


    // Logic stuff
    [HideInInspector] public bool sceneIsLoaded = false;
     public bool enemyPresent = false;
    [HideInInspector] public float lastTime = 0;
    [HideInInspector] public double timeStamp;
    [HideInInspector] public Vector2 spawnBounds;
    // Lists
     public List<Food> activeFoods;
     public int maxFood = 1;
     public Dictionary<string, Fish> fishes;
     public Dictionary<string, Fish> utilityFish;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(Camera.main);
            Destroy(playerObj);
            Destroy(eventManager);
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneHasLoaded;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        spawnBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        activeFoods = new List<Food>();
        fishes = new Dictionary<string, Fish>();
        utilityFish = new Dictionary<string, Fish>();

        DontDestroyOnLoad(playerObj);
        player = playerObj.GetComponent<Player>();

        DontDestroyOnLoad(audioObj);
        audioManager = audioObj.GetComponent<AudioManager>();

        DontDestroyOnLoad(eventManager);

        DontDestroyOnLoad(adManagerObj);
        adManager = adManagerObj.GetComponent<AdsManager>();

        DontDestroyOnLoad(goldSpawner);
        DontDestroyOnLoad(fishSpawner);
        DontDestroyOnLoad(foodSpawner);
    }

    /// <summary>
    /// Check continually for events
    /// </summary>
    void Update()
    {
        if (Random.value < 0.0005f)
        {
            PlaySound("Bubbles");
        }

        if (enemyPresent && Input.GetKeyDown(KeyCode.Mouse0))
            PlaySound("Shot");

        //if (Input.GetKeyDown(KeyCode.Tab))
        //{

        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Application.Quit();
        //}
    }

    private void SceneHasLoaded(Scene s, LoadSceneMode mode)
    {
        StartCoroutine(SceneLoadLate(s));
    }
    private IEnumerator SceneLoadLate(Scene s)
    {
        // Delay to be absolutely sure the scene has loaded
        yield return new WaitForSeconds(.01f);

        // Do some scene specific operations
        switch (s.name)
        {
            case "MainMenu":
                foodSpawner.isSpawningFood = false;
                break;
            case "Game":
                foodSpawner.isSpawningFood = false;
                break;
            case "Aquarium":
                foreach (Fish fish in FindObjectsOfType<Fish>())
                {
                    fishes.Add(fish.name, fish);
                }
                foodSpawner.isSpawningFood = true;
                break;
        }



        // Tell all classes that the scene is loaded
        sceneIsLoaded = true;
    }






    // ----------------------------------------------------------------------------------------------------------------------------------------
    // Helpers for all scripts


    ///// <summary>
    ///// Fade in the scene (must already be blacked out)
    ///// </summary>
    //public IEnumerator FadeIn()
    //{
    //    faderOverlayAnim.SetTrigger("FadeIn");
    //    yield return new WaitUntil(IsTransparent);
    //}
    //private bool IsTransparent()
    //{ return faderOverlayAnim.GetCurrentAnimatorStateInfo(0).IsName("Transparent"); }
    ///// <summary>
    ///// Fade out the scene (must have scene visible)
    ///// </summary>
    //public IEnumerator FadeOut()
    //{
    //    faderOverlayAnim.SetTrigger("FadeOut");
    //    yield return new WaitUntil(IsOpaque);
    //}
    //private bool IsOpaque()
    //{ return faderOverlayAnim.GetCurrentAnimatorStateInfo(0).IsName("Opaque"); }

    public void PlaySound(string soundName)
    {
        audioManager.PlaySound(soundName);
    }

    /// <summary>
    /// Given the player has enough money, spawn the given fish
    /// </summary>
    /// <param name="fishPrefab">Fish prefab to spawn</param>
    public void SpawnFish(GameObject fishPrefab)
    {
        // Verify player has enough money
        if (!enemyPresent && player.gold >= fishPrefab.GetComponent<Fish>().cost)
        {
            // Spend the money for the fish
            player.SpendGold(fishPrefab.GetComponent<Fish>().cost);
            // Spawn the fish
            GameObject fishObj = fishSpawner.SpawnFish(fishPrefab);
            // Add it to fish dictionary
            if (!fishes.TryAdd(fishObj.GetComponent<Fish>().name, fishObj.GetComponent<Fish>()))
            {
                Debug.Log("Couldn't add fish :/");
            }
        }

        // Else do nothing. Maybe add some announcement that the player doesn't have enough money
    }
    /// <summary>
    /// Attempt to kill a given fish
    /// </summary>
    /// <param name="fish">Fish to kill</param>
    public void KillFish(GameObject fish)
    {
        if (fishes.Remove(fish.GetComponent<Fish>().name))
        {
            Destroy(fish);
        }

        // Maybe some error that the fish doesn't exist??
    }

    /// <summary>
    /// Spawn in a given enemy prefab
    /// </summary>
    /// <param name="enemyPrefab">Enemy prefab to spawn</param>
    public void SpawnEnemy(GameObject enemyPrefab)
    {
        // Verify there are fish present
        if (fishes.Count > 0)
        {
            // Set booleans
            foodSpawner.isSpawningFood = false;
            enemyPresent = true;
            // Spawn enemy
            enemySpawner.SpawnEnemy(enemyPrefab);
            foreach (KeyValuePair<string, Fish> fish in fishes)
                fish.Value.OnEnemySpawn();
        }
    }
    /// <summary>
    /// Kill the given enemy
    /// </summary>
    public async void KillEnemy(GameObject enemy)
    {
        // Despawn the enemy
        Vector3 position = enemy.transform.position;
        int value = enemy.GetComponent<Enemy>().killReward;
        Destroy(enemy);

        // Small delay (in future, be the enemy death animation)
        await Task.Delay(1000);

        SpawnGold(position, value);
        enemyPresent = false;
        foodSpawner.isSpawningFood = true;
        foreach (KeyValuePair<string, Fish> fish in fishes)
            fish.Value.OnEnemyKilled();
    }

    /// <summary>
    /// Spawn a piece of collectable gold
    /// </summary>
    /// <param name="spawnLocation">Location of the spawn</param>
    /// <param name="value">How much the piece is worth</param>
    public void SpawnGold(Vector3 spawnLocation, int value)
    {
        goldSpawner.SpawnGold(goldPrefab, spawnLocation, value);
    }

    
}
