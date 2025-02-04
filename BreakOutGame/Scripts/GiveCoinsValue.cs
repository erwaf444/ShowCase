using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiveCoinsValue : MonoBehaviour
{
    public TextMeshProUGUI giveCoins;

    public void Start()
    {
        int coins = StaticData.valueToKeep;
        giveCoins.text = coins.ToString("00000");
    }
}
