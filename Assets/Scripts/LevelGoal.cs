using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Level", order = 1,menuName ="ScriptableObjects/Level")]
public class LevelGoal : ScriptableObject
{
    public Sprite rewardIcon;
    public int requiredExp;
    public int rewardCount;
    public int rewardIndex;
    public enum Type
    {
        money,
        space,
        seed
    }

    public Type type;
    public bool isPaid; //<- if we have paid rewards

    public void GetReward()
    {
        switch (type)
        {
            case Type.money:
                GardenManager.money += rewardCount;
                break;
            case Type.space:
                GVar.gardenSize += 1;
                break;
            case Type.seed:
                for (int i = 0; i < rewardCount; i++)
                {
                    GVar.playerSeedsIndex.Add(rewardIndex);
                }
                break;
            default:
                break;
        }
    }

}
