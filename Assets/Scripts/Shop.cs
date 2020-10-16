using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject popUp;
    public GameObject seedPage;
    public GameObject frontPage;
    public GameObject spacePage;

    public Button moka;
    public Button buga;
    public Button sogo;

    public Sprite available;
    public Sprite unAvailable;
    public static bool gardenButton = false;
    public Text moneyText;
    // In shop: standard amount of money is 40, when going back to the garden money is updated, but when going back to the shop money is again 40

    private void Start()
    {
        popUp.SetActive(false);
        seedPage.SetActive(false);
        frontPage.SetActive(true);
        spacePage.SetActive(false);
        moneyText.text = GardenManager.money.ToString();
    }

    private void Update()
    {
        moneyText.text = GardenManager.money.ToString();
        if (GardenManager.money < 20)
        {
            moka.GetComponent<Image>().sprite = unAvailable;
            moka.GetComponent<Button>().interactable = false;
        }
        if (GardenManager.money < 50)
        {
            buga.GetComponent<Image>().sprite = unAvailable;
            buga.GetComponent<Button>().interactable = false;
        }
        if (GardenManager.money < 200)
        {
            sogo.GetComponent<Image>().sprite = unAvailable;
            sogo.GetComponent<Button>().interactable = false;
        }
    }

    public void BuySeed10()
    {
        if (GardenManager.money >= 20)
        {
            moka.GetComponent<Image>().sprite = available;
            GVar.playerSeedsIndex.Add(0);
            GardenManager.money -= 20;
        }/*
        else
        {
            
        }*/
    }

    public void BuySeed20() { 
        if (GardenManager.money >= 50)
        {
            buga.GetComponent<Image>().sprite = available;
            GVar.playerSeedsIndex.Add(1);
            GardenManager.money -= 50;
        }/*
        else
        {
            
        }*/
    }

    public void BuySeed30()
    {
        if (GardenManager.money >= 200)
        {
            sogo.GetComponent<Image>().sprite = available;
            GVar.playerSeedsIndex.Add(2);
            GardenManager.money -= 200;
        }/*
        else
        {
            
        }*/
    }
        
    /*public void BuySeed40()
    {
        if (GardenManager.money >= 40)
        {
            // add this seed to inventory
            GardenManager.money -= 40;
        }
        else
        {
            popUp.SetActive(true);
        }
    }

    public void BuySeed50()
    {
        if (GardenManager.money >= 50)
        {
            // add this seed to inventory
            GardenManager.money -= 50;
        }
        else
        {
            popUp.SetActive(true);
        }
    }
        
    public void BuySeed60()
    {
        if (GardenManager.money >= 60)
        {
            // add this seed to inventory
            GardenManager.money -= 60;
        }
        else
        {
            popUp.SetActive(true);
        }
    }
        
    public void BuySeed70()
    {
        if (GardenManager.money >= 70)
        {
            // add this seed to inventory
            GardenManager.money -= 70;
        }
        else
        {
            popUp.SetActive(true);
        }
    }
        
    public void BuySeed80()
    {
        if (GardenManager.money >= 80)
        {
            // add this seed to inventory
            GardenManager.money -= 80;
        }
        else
        {
            popUp.SetActive(true);
        }
    }*/

    public void popUpClose()
    {
        popUp.SetActive(false);
    }
    
    public void backButton()
    {
        gardenButton = true;
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
        SceneManager.LoadScene("Garden");
    }

    public void OpenSeedPage()
    {
        seedPage.SetActive(true);
        frontPage.SetActive(false);
    }

    public void BackToFront()
    {
        seedPage.SetActive(false);
        frontPage.SetActive(true);
        spacePage.SetActive(false);
    }

}
