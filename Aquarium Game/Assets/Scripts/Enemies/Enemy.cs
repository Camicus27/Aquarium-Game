using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Tween tween;
    public SpriteRenderer sprite;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float attackingSpeed;
    [SerializeField] private float attackCooldown;
    private float lastFishConsumptionTime;
    private float lastPlayerAttackTime;
    private bool isWandering = true;
    private bool isChasingFish;
    private bool isAttackingFish;
    private GameObject fishToEat;

    [HideInInspector] public new string name;
    [HideInInspector] public string fishName;
    [HideInInspector] public string description;
    [HideInInspector] public int killReward;
    [HideInInspector] public int level;
    [HideInInspector] public int damage;
    [HideInInspector] public float attackRate;
    [HideInInspector] public int health;


    protected void Start()
    {
        name = "Enemy";
        fishName = "Enemy1";
        description = "blank";
        killReward = 150;
        level = 1;
        damage = 15;
        attackRate = 1f;
        health = 250;

        lastFishConsumptionTime = Time.time - (attackCooldown * 0.5f);
        lastPlayerAttackTime = Time.time;
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
            // Generates a random x,y point in the tank
            // Generate an x position opposite of self
            float x;
            if (transform.position.x < 0)
                x = Random.Range(.69f, GameManager.instance.spawnBounds.x - .5f);
            else
                x = Random.Range(-GameManager.instance.spawnBounds.x + .5f, -.69f);
            // Generate a y position relatively close to self
            float y = Random.Range(transform.position.y - .75f, transform.position.y + .75f);
            // Boundary checks
            if (y > GameManager.instance.spawnBounds.y - 3.33f)
                y = GameManager.instance.spawnBounds.y - 3.33f;
            else if (y < -GameManager.instance.spawnBounds.y + 2f)
                y = -GameManager.instance.spawnBounds.y + 2f;

            // Used for detecting when to flip the sprite based on current vs desired position
            if (x < transform.position.x && sprite.flipX || x > transform.position.x && !sprite.flipX)
                sprite.flipX = !sprite.flipX;

            // DOTween to the new position
            tween = transform.DOMove(new Vector3(x, y, 0), 2f).SetEase(Ease.InOutCubic);
            await tween.AsyncWaitForCompletion();
        }
    }

    /// <summary>
    /// Detect fish, pathfind to fish if applicable
    /// </summary>
    private void Update()
    {
        // Wander until hungry for a fish
        if (!isChasingFish)
        {
            // Detect if there is fish
            if (GameManager.instance.fishes.Count > 0)
            {
                // Cooldown buffer
                if (Time.time > lastFishConsumptionTime + attackCooldown)
                {
                    // Find any arbitrary fish
                    fishToEat = GameObject.FindGameObjectWithTag("Fish");
                    // Find the largest existing fish and target that fish
                    foreach (KeyValuePair<string, Fish> fish in GameManager.instance.fishes)
                    {
                        if (fish.Value.gameObject.transform.localScale.x > fishToEat.gameObject.transform.localScale.x)
                            fishToEat = fish.Value.gameObject;
                    }

                    // Stop wandering, chase fish
                    isWandering = false;
                    if (tween.IsPlaying()) { tween.Kill(); }
                    isChasingFish = true;
                }
            }
        }
        // Chase current target fish
        else
        {
            // Used for detecting when to flip the sprite based on current vs desired position
            if (transform.position.x < fishToEat.transform.position.x && !sprite.flipX || transform.position.x > fishToEat.transform.position.x && sprite.flipX)
                sprite.flipX = !sprite.flipX;

            // Pathfinding for chasing fish (chase speed or attacking speed)
            if (isAttackingFish)
                transform.position = Vector3.Lerp(transform.position, fishToEat.transform.position, attackingSpeed * Time.deltaTime);
            else
                transform.position = Vector3.Lerp(transform.position, fishToEat.transform.position, chaseSpeed * Time.deltaTime);
                
        }



        // Gold spawn rate
        /*if (!isStarving && Time.time > lastGoldSpawnTime + goldSpawnRate + Random.Range(-0.5f, 2f))
        {
            lastGoldSpawnTime = Time.time;
            GameManager.instance.SpawnGold(transform.position + Vector3.down, goldValue);
        }*/

        // Starvation detection
        /*if (Time.time > lastFoodConsumptionTime + starvationRate)
        {
            StartCoroutine(Starvation());
            isStarving = true;
            lastFoodConsumptionTime = -20;
        }*/
    }

    /// <summary>
    /// Callback when enemy touches the fish to eat
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the trigger is the fish to eat
        if (!isAttackingFish && collision.gameObject.tag == "Fish" && collision.gameObject == fishToEat)
        {
            Debug.Log("Fish trigger hit, begin attack!");
            // Start attacking the fish while still in range
            isAttackingFish = true;
            StartCoroutine(Attack(fishToEat.GetComponent<Fish>()));

            // Also do some animation of eating the fish and some sound effect
        }
    }

    /// <summary>
    /// Callback when fish is out of range
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the trigger is the fish to eat
        if (isAttackingFish && collision.gameObject.tag == "Fish" && collision.gameObject == fishToEat)
        {
            Debug.Log("Fish trigger left, stop attacking, start small cooldown!");
            // Chase at a slower pace to give the fish a chance to escape for short time
                //lastFishConsumptionTime = Time.time + (attackCooldown * .25f);
            StartCoroutine(EscapeLeeway());
        }
    }
    private IEnumerator EscapeLeeway()
    {
        yield return new WaitForSeconds(attackCooldown * .25f);
        isAttackingFish = false;
    }

    /// <summary>
    /// Coroutine to attack a fish while in range
    /// </summary>
    private IEnumerator Attack(Fish fish)
    {
        // While fish is still alive and attacking
        while (isAttackingFish && fish.health >= 0)
        {
            // Deal attack damage and delay by attack rate
            fish.health -= damage;
            Debug.Log("Attack registered! New fish hp: " + fish.health);
            yield return new WaitForSeconds(attackRate);
        }

        // If the fish has been killed
        if (fish.health <= 0)
        {
            Debug.Log("Fish eaten. Final fish hp: " + fish.health);

            // Return to a default wander state to cooldown
            isAttackingFish = false;
            isChasingFish = false;
            isWandering = true;
            Wander();

            // Kill the fish
            GameManager.instance.KillFish(fishToEat);
            lastFishConsumptionTime = Time.time;
            fishToEat = null;
        }
    }

    /// <summary>
    /// When player clicks on the enemy, deal damage to the enemy
    /// </summary>
    private void OnMouseDown()
    {
        if (Time.time > lastPlayerAttackTime + GameManager.instance.player.attackCooldown)
        {
            lastPlayerAttackTime = Time.time;
            health -= GameManager.instance.player.damage;
            // Flash color to portray hit
            StartCoroutine(Hit());
        }

        // If the enemy has been killed
        if (health <= 0)
        {
            GameManager.instance.KillEnemy(gameObject);
        }
    }

    private IEnumerator Hit()
    {
        float green_blue = .65f;
        sprite.color = new Color(1, green_blue, green_blue);
        while (green_blue < 1)
        {
            green_blue += .025f;
            sprite.color = new Color(1, green_blue, green_blue);
            yield return new WaitForFixedUpdate();
        }
    }

    /*/// <summary>
    /// Grow the fish and increase gold drop value
    /// </summary>
    public async void Grow()
    {
        // Increase gold drop value
        goldValue += goldIncrement;
        growthLevel++;
        // Tween a small growth of sprite
        await transform.DOScale(new Vector3(transform.localScale.x + 0.014f, transform.localScale.y + 0.014f), .75f).SetEase(Ease.OutBounce).AsyncWaitForCompletion();
    }*/

    /*/// <summary>
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
            Destroy(gameObject);
        }
    }*/
}
