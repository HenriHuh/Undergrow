﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Consumable items like keys, chests, boosts, etc
//Player items should be listed as int list by id(e.g. 1,1,3,4) 

[CreateAssetMenu(fileName ="Item", order = 1,menuName ="ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public enum Type
    {
        Chest,
        Key,
        Fertilizer,
        SuperFertilizer,
        SeedBag,
        RareSeedBag,
        Money,
        Water,
        Invincibility
    }
    public string itemName;
    public int index;
    public Type type;
    public Sprite icon;
    public int value;

    public void UseItem() 
    {
        switch (type)
        {
            case Type.Chest:
                Chest.instance.OpenChestScreen();
                break;
            case Type.Key:
                Chest.instance.OpenChestScreen();
                break;
            case Type.Fertilizer:
                InventoryHandler.instance.ActivateItem(this);
                break;
            case Type.SuperFertilizer:
                InventoryHandler.instance.AskItem(this);
                break;
            case Type.SeedBag:
                List<int> rewards = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    int r = Random.Range(0, 3);
                    rewards.Add(r);
                    GVar.playerSeedsIndex.Add(r);
                }
                InventoryHandler.instance.ShowSeedRewards(rewards);
                GVar.playerItemsIndex.Remove(ItemManager.instance.GetIndexByType(type));
                SoundManager.instance.PlaySound(SoundManager.instance.seedBagOpen);
                break;
            case Type.RareSeedBag:
                List<int> rareRewards = new List<int>();
                int[] randSeed = { 2, 4, 5 };
                for (int i = 0; i < 3; i++)
                {
                    int r = randSeed[Random.Range(0, randSeed.Length)];
                    rareRewards.Add(r);
                    GVar.playerSeedsIndex.Add(r);
                }
                InventoryHandler.instance.ShowSeedRewards(rareRewards);
                GVar.playerItemsIndex.Remove(ItemManager.instance.GetIndexByType(type));
                SoundManager.instance.PlaySound(SoundManager.instance.seedBagOpen);
                break;
            case Type.Money:
                break;
            default:
                break;
        }
        GVar.itemsUsed++;
        AnalyticsManager.SendEvent(AnalyticsManager.EventType.Custom, AnalyticsManager.EventName.item_use);
        InventoryHandler.instance.UpdateUI();
    }
}
