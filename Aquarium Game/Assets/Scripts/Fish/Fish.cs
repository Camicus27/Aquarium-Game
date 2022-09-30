using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Fish : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Tween tween;
    private GameObject food;
    public SpriteRenderer sprite;
    [SerializeField] private float speed;
    [SerializeField] private float foodTriggerRange;
    [SerializeField] private float foodCooldown;
    private float lastFoodConsumptionTime;
    private float lastGoldSpawnTime;
    private bool isWandering = true;
    private bool isChasingFood;
    private bool isScattering;
    private bool isStarving;

     public new string name;
    [HideInInspector] public string fishName;
    [HideInInspector] public string description;
    [HideInInspector] public int health;
     public int cost;
    [HideInInspector] public int goldSpawnRate;
    [HideInInspector] public int goldValue;
    [HideInInspector] public int goldIncrement;
    [HideInInspector] public int foodToLevelUp;
    [HideInInspector] public int growthLevel;
    [HideInInspector] public int maxLevel;
    [HideInInspector] public int foodEaten;
    [HideInInspector] public int starvationRate;
    

    protected void Start()
    {
        lastFoodConsumptionTime = Time.time - foodCooldown;
        lastGoldSpawnTime = Time.time;
        rigidBody = GetComponent<Rigidbody2D>();
        Wander();
    }

    /// <summary>
    /// Asynchronously perform a random wander to a new location until isWander == false
    /// </summary>
    protected async void Wander()
    {
        while (isWandering)
        {
            // Generate a random x,y point in the tank and determine the distance to it
            float x = Random.Range(-GameManager.instance.spawnBounds.x + .5f, GameManager.instance.spawnBounds.x - .5f);
            float y = Random.Range(-GameManager.instance.spawnBounds.y + 2f, GameManager.instance.spawnBounds.y - 3.33f);
            float distance = Vector2.Distance(new Vector2(x, y), transform.position);

            // Used for detecting when to flip the sprite based on current vs desired position
            if (x < transform.position.x && sprite.flipX || x > transform.position.x && !sprite.flipX)
                sprite.flipX = !sprite.flipX;

            // DOTween to the new position
            tween = transform.DOMove(new Vector3(x, y, 0), Random.Range(distance, distance * 1.5f)).SetEase(Ease.InOutSine, 2f);
            await tween.AsyncWaitForCompletion();
        }
    }

    /// <summary>
    /// Asynchronously perform a random run to a new location until isScattering == false
    /// </summary>
    protected async void Scatter()
    {
        while (isScattering)
        {
            // Generate a random x,y point in the tank and determine the distance to it
            float x = Random.Range(-GameManager.instance.spawnBounds.x + .5f, GameManager.instance.spawnBounds.x - .5f);
            float y = Random.Range(-GameManager.instance.spawnBounds.y + 2f, GameManager.instance.spawnBounds.y - 3.33f);
            float distance = Vector2.Distance(new Vector2(x, y), transform.position);

            // Used for detecting when to flip the sprite based on current vs desired position
            if (x < transform.position.x && sprite.flipX || x > transform.position.x && !sprite.flipX)
                sprite.flipX = !sprite.flipX;

            // DOTween to the new position
            tween = transform.DOMove(new Vector3(x, y, 0), Random.Range(distance * .25f, distance * .4f)).SetEase(Ease.OutSine, 1f);
            await tween.AsyncWaitForCompletion();
        }
    }

    /// <summary>
    /// Detect food, pathfind to food if applicable, spawn gold, check hunger status
    /// </summary>
    private void Update()
    {
        // No enemy present, normal behavior
        if (!GameManager.instance.enemyPresent)
        {
            // Detect if there is food and this fish can consume food
            if (GameManager.instance.activeFoods.Count > 0 && Time.time > lastFoodConsumptionTime + foodCooldown)
            {
                // Check if this fish is close to a food and chase after it if so
                foreach (Food f in GameManager.instance.activeFoods)
                {
                    if (Vector3.Distance(f.gameObject.transform.position, transform.position) <= foodTriggerRange)
                    {
                        // Stop wandering, chase food
                        food = f.gameObject;
                        isWandering = false;
                        if (tween.IsPlaying()) { tween.Kill(); }
                        isChasingFood = true;
                        break;
                    }
                }
            }

            // Pathfinding for chasing food
            if (isChasingFood)
            {
                // Verify food still hasn't been eaten
                if (food == null)
                {
                    isChasingFood = false;
                    isWandering = true;
                    Wander();
                }
                else
                {
                    // Used for detecting when to flip the sprite based on current vs desired position
                    if (transform.position.x < food.transform.position.x && !sprite.flipX || transform.position.x > food.transform.position.x && sprite.flipX)
                        sprite.flipX = !sprite.flipX;
                    // Move towards the food
                    transform.position = Vector3.Lerp(transform.position, food.transform.position, speed * Time.deltaTime);
                }
            }

            // Gold spawn rate
            if (!isStarving && Time.time > lastGoldSpawnTime + goldSpawnRate + Random.Range(-0.25f, 5f))
            {
                lastGoldSpawnTime = Time.time;
                GameManager.instance.SpawnGold(transform.position + Vector3.down, goldValue);
            }

            // Starvation detection
            if (Time.time > lastFoodConsumptionTime + starvationRate)
            {
                StartCoroutine(Starvation());
                isStarving = true;
                lastFoodConsumptionTime = -20;
            }
        }
    }

    /// <summary>
    /// Callback when fish touches a trigger
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the trigger is food
        if (collision.gameObject.tag == "Food" && food != null)
        {
            // Consume the food and stop chasing
            //StartCoroutine(SlowDown());
            isChasingFood = false;
            food.GetComponent<Food>().OnConsume();
            GameManager.instance.PlaySound("Slurp_" + Random.Range(1,3));
            lastFoodConsumptionTime = Time.time;
            foodEaten++;
            if ((foodEaten + 1) % foodToLevelUp == 0 && growthLevel < maxLevel)
                Grow();

            isWandering = true;
            Wander();


            // Also do some animation of eating the food and some sound effect
        }
    }

    /// <summary>
    /// Grow the fish and increase gold drop value
    /// </summary>
    public async void Grow()
    {
        // Increase gold drop value
        goldValue += goldIncrement;
        growthLevel++;
        // Tween a small growth of sprite
        await transform.DOScale(new Vector3(transform.localScale.x + 0.014f, transform.localScale.y + 0.014f), .75f).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
    }

    /// <summary>
    /// Coroutine to slowly starve at a set rate. If complete, kill the fish
    /// </summary>
    private IEnumerator Starvation()
    {
        float time = Time.time;
        float red = 1;
        float blue = 1;

        while (Time.time < time + starvationRate * .33)
        {
            // Slowly change to a greenish color
            if (red > 0.3f)
            {
                red -= (Time.deltaTime * 0.05f);
                blue -= (Time.deltaTime * 0.05f);
            }

            // Check if been fed since starting starving
            if (Time.time < lastFoodConsumptionTime + 5)
            {
                isStarving = false;
                break;
            }

            sprite.color = new Color(red, 1, blue);
            yield return null;
        }

        // If completes and no longer starving, restore color, otherwise die
        if (!isStarving)
        {
            sprite.color = new Color(1, 1, 1);
        }
        else
        {
            if (tween.IsPlaying()) { tween.Kill(); }
            GameManager.instance.KillFish(gameObject);
        }
    }

    public void OnEnemySpawn()
    {
        // Give 1 minute leeway for starvation and halt gold spawning for 10 minutes
        lastGoldSpawnTime = Time.time + 600;
        lastFoodConsumptionTime = Time.time + 60;

        // Stop chasing food/wandering
        isChasingFood = false;
        isWandering = false;
        if (tween.IsPlaying()) { tween.Kill(); }


        // Start scatter
        Debug.Log("Start Scattering!!");
        isScattering = true;
        Scatter();
    }
    public void OnEnemyKilled()
    {
        // Verify enemy is still present
        if (!GameManager.instance.enemyPresent)
        {
            // Stop scattering
            isScattering = false;
            if (tween.IsPlaying()) { tween.Kill(); }

            // Resume normal behavior
            isChasingFood = false;
            isWandering = true;
            Wander();

            lastGoldSpawnTime = Time.time;
            lastFoodConsumptionTime = Time.time;
        }
    }

    public override bool Equals(object other)
    {
        if (other is GameObject)
        {
            return name == (other as GameObject).GetComponent<Fish>().name;
        }
        else if (other is Fish)
        {
            return name == (other as Fish).name;
        }
        return false;
    }

    public static bool operator ==(Fish left, Fish right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Fish left, Fish right)
    {
        return !(left.Equals(right));
    }

    private void OnDestroy()
    {
        isWandering = false;
        isScattering = false;
        if (tween.IsPlaying()) { tween.Kill(); }
    }
}
