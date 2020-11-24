using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName ="Level", order = 1,menuName ="ScriptableObjects/Level")]
public class LevelGoal : ScriptableObject
{
    public int requiredExp;
    public List<LevelReward> rewards;

    public void GetReward()
    {
        foreach (LevelReward reward in rewards)
        {
            switch (reward.type)
            {
                case LevelReward.Type.money:
                    GardenManager.money += reward.rewardCount;
                    break;
                case LevelReward.Type.space:
                    GVar.gardenSize += 1;
                    break;
                case LevelReward.Type.seed:
                    for (int i = 0; i < reward.rewardCount; i++)
                    {
                        GVar.playerSeedsIndex.Add(reward.rewardIndex);
                    }
                    if (!GVar.unlockedSeedsIndex.Contains(reward.rewardIndex))
                    {
                        GVar.unlockedSeedsIndex.Add(reward.rewardIndex);
                    }
                    break;
                default:
                case LevelReward.Type.item:
                    for (int i = 0; i < reward.rewardCount; i++)
                    {
                        GVar.playerItemsIndex.Add(reward.rewardIndex);
                    }
                    break;
            }
        }
    }

}

[System.Serializable]
public class LevelReward
{
    public int rewardCount;
    public int rewardIndex;
    public bool isPaid;
    public enum Type
    {
        money,
        space,
        seed,
        item
    }

    public Type type;


    public Sprite GetIcon()
    {
        switch (type)
        {
            case Type.money:
                return GVar.moneyIcon;
            case Type.space:
                return GVar.expansionIcon;
            case Type.seed:
                return PlantDataBase.instance.GetPlantByIndex(rewardIndex).plantVariables.seedSprite;
            case Type.item:
                return ItemManager.instance.GetItemByIndex(rewardIndex).icon;
            default:
                return null;
        }
    }
}
