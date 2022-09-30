using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool despawning;
    private bool hitTrigger;

    private void Awake()
    {
        GameManager.instance.PlaySound("DropFood");
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        GameManager.instance.foodSpawner.activeFoods.Add(this);
    }

    private void Update()
    {
        if (!hitTrigger && transform.position.y <= -GameManager.instance.spawnBounds.y + 2.55f)
        {
            hitTrigger = true;
            rb.gravityScale = 0.02f;
            StartCoroutine(DecreaseVelocity());
        }
        else if (!despawning && transform.position.y <= -GameManager.instance.spawnBounds.y + 1f)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            despawning = true;
            StartCoroutine(SlowlyDespawn());
        }
    }

    public void OnConsume()
    {
        GameManager.instance.foodSpawner.activeFoods.Remove(this);
        Destroy(gameObject);
    }

    private IEnumerator SlowlyDespawn()
    {
        yield return new WaitForSeconds(.2f);
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= 0.1f;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            yield return new WaitForFixedUpdate();
        }
        GameManager.instance.foodSpawner.activeFoods.Remove(this);
        Destroy(gameObject);
    }

    private IEnumerator DecreaseVelocity()
    {
        while (rb.velocity.y < -0.2f)
        {
            rb.velocity *= .982f;
            yield return new WaitForFixedUpdate();
        }
    }
}
