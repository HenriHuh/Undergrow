﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopButton : MonoBehaviour
{
    public void OpenShop()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
        SceneManager.LoadScene("Shop");

        Debug.Log(GardenManager.tutorialStillOn);

        if (GardenManager.tutorialStillOn == true)
        {
            GardenManager.shopOpenedTuturial = true;
        }
    }
}
