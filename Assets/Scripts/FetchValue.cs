using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//I was too lazy to assign same stuff all over again in editor

public class FetchValue : MonoBehaviour
{
    bool isActive = false;
    public Value value;
    public enum Value
    {
        PlayerLevel,
        ExpFillAmount,
        NextLevel,
        Experience,
        RequiredExperience
    }

    private void OnEnable()
    {
        StartCoroutine(WaitStart());
    }

    IEnumerator WaitStart()
    {
        yield return new WaitForEndOfFrame();
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        switch (value)
        {
            case Value.PlayerLevel:
                gameObject.GetComponent<Text>().text = (GVar.completedGoals.Count + 1).ToString();
                break;
            case Value.ExpFillAmount:
                gameObject.GetComponent<Image>().fillAmount = TaskUIHandler.instance.experienceBarFill.fillAmount;
                break;
            case Value.NextLevel:
                gameObject.GetComponent<Text>().text = (GVar.completedGoals.Count + 2).ToString();
                break;
            case Value.Experience:
                gameObject.GetComponent<Text>().text = GVar.experience.ToString() + " xp";
                break;
            case Value.RequiredExperience:
                gameObject.GetComponent<Text>().text = TaskUIHandler.instance.activeGoal.requiredExp.ToString() + " xp";
                break;
            default:
                break;
        }
    }
}
