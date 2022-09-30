using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text playerGoldCount;

    public int level;
    public int damage = 30;
    public float attackCooldown = 0.5f;
    public int gold;
    public int clams;

    public void EarnGold(int goldAmount)
    {
        gold += goldAmount;
        GoldDisplay.UpdateDisplay();
    }

    public void SpendGold(int goldAmount)
    {
        gold -= goldAmount;
        GoldDisplay.UpdateDisplay();
    }

    public void EarnClams(int clamAmount)
    {
        clams += clamAmount;
    }
}
