using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldDisplay : MonoBehaviour
{
    static private TextMeshProUGUI textDisplay;

    private void Start()
    {
        textDisplay = gameObject.GetComponent<TextMeshProUGUI>();
        UpdateDisplay();
    }

    public static void UpdateDisplay()
    {
        textDisplay.text = "$ " + GameManager.instance.player.gold;
    }
}
