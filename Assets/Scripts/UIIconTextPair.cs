using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconTextPair : MonoBehaviour
{
    public Text txtName;
    public Text txtCount;
    public Image image;

    public void SetUI(Sprite icon, string text, int count)
    {
        image.sprite = icon;
        txtCount.text = count.ToString();
        txtName.text = text;
    }

    public void SetUI(LevelReward reward, bool hasTitle)
    {
        if (hasTitle)
        {
            SetUI(reward);
        }
        else
        {
            image.sprite = reward.GetIcon();
            txtCount.text = reward.rewardCount > 1 ? reward.rewardCount.ToString() : "";
        }
    }

    public void SetUI(LevelReward reward)
    {
        image.sprite = reward.GetIcon();
        switch (reward.type)
        {
            case LevelReward.Type.money:
                txtName.text = (reward.rewardCount > 1 ? reward.rewardCount + " x " : "") + "Money";
                break;
            case LevelReward.Type.space:
                txtName.text = (reward.rewardCount > 1 ? reward.rewardCount + " x " : "") + "Garden Space";
                break;
            case LevelReward.Type.seed:
                txtName.text = (reward.rewardCount > 1 ? reward.rewardCount + " x " : "") + PlantDataBase.instance.GetPlantByIndex(reward.rewardIndex).plantVariables.plantName + " Seed";
                break;
            case LevelReward.Type.item:
                txtName.text = (reward.rewardCount > 1 ? reward.rewardCount + " x " : "") + ItemManager.instance.GetItemByIndex(reward.rewardIndex).itemName;
                break;
            default:
                break;
        }
    }
}
