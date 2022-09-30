using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDisc : Fish
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


    //public int cost;
    //public int goldSpawnRate;
    //public int goldValue;
    //public int goldIncrement;
    //public int foodToLevelUp;
    //public int maxLevel;
    //public int starvationRate;


    protected void Start()
    {
        // Speed, triggerRange, foodCooldown, sprite are defined in inspector
        // Rigidbody and timer starts are made in base.start

        cost = 100;
        goldSpawnRate = 12;
        goldValue = 15;
        goldIncrement = 5;
        foodToLevelUp = 5;
        maxLevel = 6;
        starvationRate = 45;

        base.Start();
    }

    /// Base class performs a wander unless otherwise overridden
    /// Base Update does detect food, pathfind to food if applicable, spawn gold, check hunger status
    /// Base handles eating the food on collision



}
