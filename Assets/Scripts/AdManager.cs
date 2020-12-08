using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    public enum AdType
    {
        RewardedVideo
    }

    public static AdManager instance;
    bool adFinished;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            InitAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitAds()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize("3929635", false);
    }

    public void ShowAd(AdType _type)
    {
        string p = "";
        switch (_type)
        {
            case AdType.RewardedVideo:
                GiveReward(); //Unity ads is being wonky, so player gets rewards when they press play
                p = "rewardedVideo";
                break;
            default:
                break;
        }
        Advertisement.Show(p);
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        //Prevent player from getting multiple rewards if the ad system goes bonkers

        return;

        if (adFinished) return;
        adFinished = true;
        Invoke("ResetAdTimer", 6f);

        switch (placementId)
        {
            case "rewardedVideo":
                if (showResult == ShowResult.Finished)
                {
                    GVar.playerItemsIndex.Add(ItemManager.instance.GetIndexByType(Item.Type.Key));
                    Chest.instance.adsInfoScreen.SetActive(false);
                    Chest.instance.UpdateUI();
                }
                else if (showResult == ShowResult.Failed)
                {
                    Chest.instance.adsInfo.text = "Ad failed. Try again?";
                }
                break;
            default:
                break;
        }
    }
    
    void GiveReward() //Give the player a reward because Unity ads buggy af
    {
        GVar.playerItemsIndex.Add(ItemManager.instance.GetIndexByType(Item.Type.Key));
        Chest.instance.adsInfoScreen.SetActive(false);
        Chest.instance.UpdateUI();
    }

    void ResetAdTimer()
    {
        adFinished = false;
    }
    
}
