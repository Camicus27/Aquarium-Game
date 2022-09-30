using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private Rigidbody2D rb;
    public SpriteRenderer sprite;
    private bool despawning;
    private bool hitTrigger;

    public int value;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        OnCollect();
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

    public void OnCollect()
    {
        GameManager.instance.PlaySound("CollectCoin_" + Random.Range(1, 4));
        GameManager.instance.player.EarnGold(value);
        Destroy(gameObject);
    }

    private IEnumerator SlowlyDespawn()
    {
        yield return new WaitForSeconds(1.25f);
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= 0.1f;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }

    private IEnumerator DecreaseVelocity()
    {
        while (rb.velocity.y <= -0.2f)
        {
            rb.velocity *= .982f;
            yield return new WaitForFixedUpdate();
        }
    }
}
