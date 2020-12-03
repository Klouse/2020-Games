using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text _ammoText;
    [SerializeField]
    GameObject _coinSprite;

    public int CurrentAmmo { private get; set;}
    private void Start() {
        _coinSprite.SetActive(false);
        _ammoText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _ammoText.text = "Ammo: " + CurrentAmmo;
    }

    public void EnableCoin(){
        _coinSprite.SetActive(true);
    }

    public void DisableCoin()
    {
        _coinSprite.SetActive(false);
    }

    public void EnableAmmo()
    {
        _ammoText.gameObject.SetActive(true);
    }
}
