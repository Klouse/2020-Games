using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text coinText;
    [SerializeField]
    private TMP_Text livesText;

    public void UpdateCoinUI(int coinsValue){
        coinText.text = "Coins: " + coinsValue;
    }

    public void UpdateLivesUI(int livesValue){
        livesText.text = "Lives: " + livesValue;
    }
}
