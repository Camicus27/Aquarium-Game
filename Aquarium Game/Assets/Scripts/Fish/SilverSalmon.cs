using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverSalmon : Fish
{
    //private Rigidbody2D rigidBody;
    //private Tween tween;
    //private GameObject food;
    //public SpriteRenderer sprite;
    //[SerializeField] private float speed;
    //[SerializeField] private float foodTriggerRange;
    //[SerializeField] private float foodCooldown;
    //private float lastFoodConsumptionTime;
    //private float lastGoldSpawnTime;
    //private bool isWandering = true;
    //private bool isChasingFood;
    //private bool isScared;
    //private bool isStarving;


    //public new string name;
    //public string fishName;
    //public string description;
    //public int health;
    //public int cost;
    //public int goldSpawnRate;
    //public int goldValue;
    //public int goldIncrement;
    //public int foodToLevelUp;
    //public int growthLevel;
    //public int maxLevel;
    //public int foodEaten;
    //public int starvationRate;


    protected void Start()
    {
        // Speed, triggerRange, foodCooldown, sprite are defined in inspector
        // Rigidbody and timer starts are made in base.start

        name = "Silver Salmon";
        fishName = "Silver Salmon";
        description = "Salmon has never looked shinier, or valuable!";
        health = 500;
        cost = 2500;
        goldSpawnRate = 40;
        goldValue = 300;
        goldIncrement = 50;
        foodToLevelUp = 15;
        maxLevel = 4;
        starvationRate = 150;

        base.Start();
    }

    /// Base class performs a wander unless otherwise overridden
    /// Base Update does detect food, pathfind to food if applicable, spawn gold, check hunger status
    /// Base handles eating the food on collision
}
