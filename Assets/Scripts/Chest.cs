using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script for chest opening screen

public class Chest : MonoBehaviour
{
    public static Chest instance;

    public GameObject button;
    public GameObject rewardButton;
    public Image rewardImage;
    public Text rewardCount;
    public Text chestCount;
    public Text keyCount;

    public GameObject adsInfoScreen;
    public Text adsInfo;

    [Tooltip("Possible rewards. One will be chosen.")] public List<ChestReward> rewards;

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void OpenChestScreen()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void UpdateUI()
    {
        int chestC = 0; //Chest count
        int keyC = 0; //Key count

        foreach (int i in GVar.playerItemsIndex)
        {
            Item item = ItemManager.instance.GetItemByIndex(i);
            if (item.type == Item.Type.Chest)
            {
                chestC++;
            }
            else if (item.type == Item.Type.Key)
            {
                keyC++;
            }
        }

        chestCount.text = chestC.ToString();
        keyCount.text = keyC.ToString();
    }

    public void StartAd()
    {
        AdManager.instance.ShowAd(AdManager.AdType.RewardedVideo);
    }

    public void OpenChest()
    {
        if (!GVar.playerItemsIndex.Contains(ItemManager.instance.GetIndexByType(Item.Type.Chest)))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.hitPoison);

            return;
        }
        else if (!GVar.playerItemsIndex.Contains(ItemManager.instance.GetIndexByType(Item.Type.Key)))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.hitPoison);
            adsInfo.text = "Watch ad(30s) for key?";
            adsInfoScreen.SetActive(true);
            return;
        }


        button.SetActive(false);
        rewardButton.SetActive(true);

        ChestReward reward = rewards[Random.Range(0, rewards.Count)];
        if (reward.item.type == Item.Type.Money)
        {
            GardenManager.money += reward.count;
            GardenManager.instance.UpdateMoneyVisual();
        }
        else
        {
            for (int i = 0; i < reward.count; i++)
            {
                GVar.playerItemsIndex.Add(reward.item.index);
            }
        }
        rewardImage.sprite = reward.item.icon;
        rewardCount.text = reward.count.ToString();

        GVar.playerItemsIndex.Remove(ItemManager.instance.GetIndexByType(Item.Type.Chest));
        GVar.playerItemsIndex.Remove(ItemManager.instance.GetIndexByType(Item.Type.Key));
        UpdateUI();
        SoundManager.instance.PlaySound(SoundManager.instance.chestOpen);
        InventoryHandler.instance.UpdateUI();
    }

    public void CloseChest()
    {
        rewardButton.SetActive(false);
        button.SetActive(true);
        if (!GVar.playerItemsIndex.Contains(ItemManager.instance.GetIndexByType(Item.Type.Chest)) && !GVar.playerItemsIndex.Contains(ItemManager.instance.GetIndexByType(Item.Type.Key)))
        {
            gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class ChestReward
{
    public Item item;
    public int count;
}
