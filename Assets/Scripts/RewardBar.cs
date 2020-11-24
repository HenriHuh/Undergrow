using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardBar : MonoBehaviour
{
    public Text levelText;
    public GameObject RewardPrefab;
    public Transform freeRewardParent;
    public Transform paidRewardParent;

    public void UpdateValues(List<LevelReward> rewards, int level)
    {
        levelText.text = "Lv " + level.ToString();

        //List stuff for possible pop up
        List<Sprite> icons = new List<Sprite>();
        List<string> descs = new List<string>();
        foreach (LevelReward reward in rewards)
        {
            icons.Add(reward.GetIcon());
            switch (reward.type)
            {
                case LevelReward.Type.money:
                    descs.Add("Money x " + reward.rewardCount);
                    break;
                case LevelReward.Type.space:
                    descs.Add("New garden space");
                    break;
                case LevelReward.Type.seed:
                    descs.Add(PlantDataBase.instance.GetPlantByIndex(reward.rewardIndex).plantVariables.plantName + (reward.rewardCount > 1 ? " x " + reward.rewardCount : ""));
                    break;
                case LevelReward.Type.item:
                    descs.Add(ItemManager.instance.GetItemByIndex(reward.rewardIndex).itemName + (reward.rewardCount > 1 ? " x " + reward.rewardCount : ""));
                    break;
                default:
                    break;
            }
        }
        gameObject.GetComponent<Button>().onClick.AddListener(() => InfoPopUp.instance.Open(icons, descs, transform));

        //Make actual UI
        foreach (LevelReward reward in rewards)
        {
            if (!reward.isPaid)
            {
                GameObject g = Instantiate(RewardPrefab, freeRewardParent);
                g.GetComponent<UIIconTextPair>().SetUI(reward, false);
            }
            else
            {
                GameObject g = Instantiate(RewardPrefab, paidRewardParent);
                g.GetComponent<UIIconTextPair>().SetUI(reward, false);
            }
        }
    }

}
