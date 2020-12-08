using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is for handling seed buying UI

public class BuyButton : MonoBehaviour
{
    public Text txtPrice;
    public Text txtName;
    public Text txtOwned;
    public Image itemImg;
    public Image itemImgOwned;
    public GameObject lockedOverlay;
    public GameObject lockedIcon;
    [HideInInspector] public Plant plant;

    public void SetUI(Plant _plant)
    {
        plant = _plant;
        SetUI();
    }

    public void SetUI()
    {
        //Check amount of owned seeds of this type
        int owned = 0;
        foreach (int i in GVar.playerSeedsIndex)
        {
            if (i == plant.index) owned++;
        }

        //Assign values
        txtPrice.text = plant.plantVariables.value.ToString();
        txtName.text = plant.plantVariables.plantName;
        itemImg.sprite = plant.plantVariables.seedSprite;
        itemImgOwned.sprite = plant.plantVariables.seedSprite;
        txtOwned.text = owned.ToString();


        //Check money
        if (GardenManager.money >= plant.plantVariables.value)
        {
            lockedOverlay.SetActive(false);
        }
        else
        {
            lockedOverlay.SetActive(true);
        }

        //Check if unlocked
        if (GVar.unlockedSeedsIndex.Contains(plant.index))
        {
            lockedIcon.SetActive(false);
        }
        else
        {
            lockedIcon.SetActive(true);
            lockedOverlay.SetActive(true);
        }

        //Check if player has no seed
        if (owned == 0)
        {
            itemImgOwned.color = Color.gray;
        }
        else
        {
            itemImgOwned.color = Color.white;
        }
    }
}
