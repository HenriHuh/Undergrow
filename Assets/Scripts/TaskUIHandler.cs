using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TaskUIHandler : MonoBehaviour
{
    public static TaskUIHandler instance;

    public GameObject taskButtonPrefab;
    public GameObject levelIconPrefab;
    public Transform taskParent;
    public Transform levelParent;
    public RectTransform content;
    public Transform rewardsParent;
    public GameObject rewardPrefab;
    public Image experienceBarFill;
    public Text currentLevelText;
    public Text nextLevelText;
    public GameObject levelRewardBar;
    public Transform levelRewardParent;

    public Sprite enabledButtonSprite;
    public Sprite disabledButtonSprite;
    public GameObject goalPopPrefab;
    public GameObject goalRewardPrefab;

    float tempExp;
    List<GameObject> goalObjects = new List<GameObject>();
    List<LevelGoal> levelGoals = new List<LevelGoal>();
    //GameObject activeObject;
    [HideInInspector] public LevelGoal activeGoal;
    LevelGoal prevGoal;
    bool completed;
    Vector3 levelParentTargetPos;
    Vector3 levelParentOrigin;


    //TODO: 
    //  1. Figure out why scrollbar handle is dumb

    void Start()
    {
        instance = this;

        List<GameTask> toList = TaskManager.instance.GetOrderedTasks();
        

        foreach (GameTask t in toList)
        {

            GameObject g = Instantiate(taskButtonPrefab, taskParent);
            g.GetComponent<Button>().onClick.AddListener(() => SelectTask(t, g));

            string requires = t.description;
            foreach (TaskRequirement req in t.requirements)
            {
                if (req.type == TaskRequirement.Type.flower)
                {

                    int count = 0;
                    if (GVar.harvestedFlowers.ContainsKey(req.reqId))
                    {
                        count = GVar.harvestedFlowers[req.reqId];
                    }

                    requires += count + "/" + req.collectableAmount + " <sprite=" + req.reqId + ">";
                }
                else if (req.type == TaskRequirement.Type.plant)
                {
                    int count = 0;
                    if (GVar.plantedFlowers.ContainsKey(req.reqId))
                    {
                        count = GVar.plantedFlowers[req.reqId];
                    }

                    //TODO:
                    //  Make sure that this is correct in the text asset
                    //  Right now adds the plant ids before flower ids
                    //  this might work:
                    //requires += count + "/" + req.collectableAmount + " <sprite=" + (PlantDataBase.instance.flowers.Count - 1 + req.reqId) + ">";
                    requires += count + "/" + req.collectableAmount + " " + PlantDataBase.instance.GetPlantByIndex(req.reqId).plantVariables.plantName;
                }
            }

            g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = requires;
            g.transform.GetChild(2).GetComponent<Text>().text = t.rewardXp + "xp";
            if (t.isDaily)
            {
                g.transform.GetChild(4).gameObject.SetActive(true);
            }
            foreach (TaskRequirement tr in t.requirements)
            {
                if (tr.type == TaskRequirement.Type.flower && (!GVar.harvestedFlowers.ContainsKey(tr.reqId) || GVar.harvestedFlowers[tr.reqId] < tr.collectableAmount))
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = disabledButtonSprite;
                    break;
                }
                else if (tr.type == TaskRequirement.Type.plant && (!GVar.plantedFlowers.ContainsKey(tr.reqId) || GVar.plantedFlowers[tr.reqId] < tr.collectableAmount))
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = disabledButtonSprite;
                    break;
                }
                else if (tr.type != TaskRequirement.Type.flower && tr.type != TaskRequirement.Type.plant && !GVar.completedTaskTypes.Contains(tr.type))
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = disabledButtonSprite;
                    break;
                }

            }
        }

        //TODO:
        //  1. Add level rewards in seperate screen and crap

        foreach (LevelGoal lvl in LevelManager.instance.levels)
        {
            //GameObject g = Instantiate(levelIconPrefab, levelParent);
            //g.GetComponent<Image>().sprite = lvl.rewardIcon;
            //g.GetComponentInChildren<Text>().text = lvl.rewardCount > 1 ? lvl.rewardCount.ToString() : "";
            //goalObjects.Add(g);
            levelGoals.Add(lvl);
            if (GVar.experience >= lvl.requiredExp)
            {
                prevGoal = lvl;
                //g.transform.GetChild(2).GetComponent<Image>().fillAmount = 1;

                if (!GVar.completedGoals.Contains(LevelManager.instance.GetIndex(lvl)))
                {
                    GVar.completedGoals.Add(LevelManager.instance.GetIndex(lvl));
                    lvl.GetReward();
                    //g.GetComponent<Animator>().SetTrigger("pop");
                }
                //else
                //{
                //    Destroy(g.GetComponent<Animator>());
                //    g.GetComponent<Image>().color = Color.clear;
                //    g.GetComponentInChildren<Text>().text = "";
                //}
            }
            else if (activeGoal == null)
            {
                //activeObject = g;
                activeGoal = lvl;
                //g.transform.GetChild(2).GetComponent<Image>().fillAmount = prevGoal == null 
                    //? (float)GVar.experience / activeGoal.requiredExp : (float)(GVar.experience - prevGoal.requiredExp) / activeGoal.requiredExp; 
            }
        }

        //All levels completed
        if (activeGoal == null)
        {
            completed = true;
            activeGoal = levelGoals[levelGoals.Count - 1];
        }
        else
        {
            foreach (LevelReward reward in activeGoal.rewards)
            {
                GameObject g = Instantiate(rewardPrefab, rewardsParent);
                g.GetComponent<UIIconTextPair>().SetUI(reward);
            }
        }

        levelParentOrigin = levelParent.transform.position;
        UpdateUI();

        int i = 2;
        foreach (LevelGoal goal in LevelManager.instance.levels)
        {
            GameObject g = Instantiate(levelRewardBar, levelRewardParent);
            g.GetComponent<RewardBar>().UpdateValues(goal.rewards, i);
            i++;
        }
    }


    void Update()
    {
        levelParent.transform.position = Vector3.Lerp(levelParent.transform.position, levelParentTargetPos, Time.deltaTime * 5);

        if (completed) return;

        float target = prevGoal == null
            ? (float)GVar.experience / activeGoal.requiredExp : (float)(GVar.experience - prevGoal.requiredExp) / (float)(activeGoal.requiredExp - prevGoal.requiredExp);
        target = target > activeGoal.requiredExp ? activeGoal.requiredExp : target;

        tempExp = Mathf.Lerp(tempExp, target, Time.deltaTime * 2);
        experienceBarFill.fillAmount = tempExp;
        //activeObject.transform.GetChild(2).GetComponent<Image>().fillAmount = tempExp;


        if (GVar.experience >= activeGoal.requiredExp && tempExp > 0.95f)
        {
            NextGoal();
            UpdateUI();
        }
    }


    public void OpenRewardInfo() //Open an info box when level rewards is clicked
    {
        List<Sprite> icons = new List<Sprite>();
        List<string> descs = new List<string>();
        foreach (LevelReward reward in activeGoal.rewards)
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
        InfoPopUp.instance.Open(icons, descs, rewardsParent);
    }

    bool CheckCompleted(GameTask t)
    {
        foreach (TaskRequirement tr in t.requirements)
        {
            if (!GVar.harvestedFlowers.ContainsKey(tr.reqId) || GVar.harvestedFlowers[tr.reqId] < tr.collectableAmount)
            {
                return false;
            }
        }
        return true;
    }

    void NextGoal()
    {

        if (!GVar.completedGoals.Contains(levelGoals.IndexOf(activeGoal)))
        {
            activeGoal.GetReward();
            GVar.completedGoals.Add(LevelManager.instance.GetIndex(activeGoal));
            SoundManager.instance.PlaySound(SoundManager.instance.completeLevel);

            //Level up

            //Call the info window
            /*List<Sprite> sprites = new List<Sprite>();
            sprites.Add(activeGoal.rewardIcon);
            sprites.Add(activeGoal.rewardIcon);
            List<string> infs = new List<string>();
            infs.Add(" Don't mind this window.");
            infs.Add(" Im just testing the animation.");
            InfoPopUp.instance.Open(sprites, infs);
            */

            GameObject popup = Instantiate(goalPopPrefab, transform);
            popup.transform.GetChild(0).GetComponent<Text>().text = "Level " + (GVar.completedGoals.Count + 1) + " Reached";
            foreach (LevelReward reward in activeGoal.rewards)
            {
                GameObject g = Instantiate(goalRewardPrefab, popup.GetComponentInChildren<HorizontalLayoutGroup>().transform);
                g.GetComponent<Image>().sprite = reward.GetIcon();
                g.GetComponentInChildren<Text>().text = reward.rewardCount > 1 ? reward.rewardCount.ToString() : "";
            }

            //activeObject.GetComponent<Animator>().SetTrigger("pop");
        }

        int index = levelGoals.IndexOf(activeGoal) + 1;
        if (index < levelGoals.Count)
        {
            activeGoal = levelGoals[index];
            //activeObject = goalObjects[index];
        }
        else
        {
            completed = true;
        }
        tempExp = 0;

        foreach (Transform t in rewardsParent.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (LevelReward reward in activeGoal.rewards)
        {
            GameObject g = Instantiate(rewardPrefab, rewardsParent);
            g.GetComponent<UIIconTextPair>().SetUI(reward);
        }
    }

    void UpdateUI()
    {
        currentLevelText.text = GVar.completedGoals.Count.ToString();
        nextLevelText.text = (GVar.completedGoals.Count + 1).ToString();
        levelParentTargetPos = levelParentOrigin;
        for (int i = 2; i < LevelManager.instance.GetIndex(activeGoal); i++)
        {
            levelParentTargetPos = levelParentTargetPos + Vector3.left * Screen.width/4;
        }
    }

    public void SelectTask(GameTask task, GameObject btn)
    {
        foreach (TaskRequirement tr in task.requirements)
        {
            if (tr.type == TaskRequirement.Type.flower && (!GVar.harvestedFlowers.ContainsKey(tr.reqId) || GVar.harvestedFlowers[tr.reqId] < tr.collectableAmount))
            {
                return;
            }
            else if (tr.type == TaskRequirement.Type.plant && (!GVar.plantedFlowers.ContainsKey(tr.reqId) || GVar.plantedFlowers[tr.reqId] < tr.collectableAmount))
            {
                return;
            }
            else if (tr.type != TaskRequirement.Type.flower && tr.type != TaskRequirement.Type.plant && !GVar.completedTaskTypes.Contains(tr.type))
            {
                return;
            }
        }
        SoundManager.instance.PlaySound(SoundManager.instance.completeTask);
        GVar.experience += task.rewardXp;
        if (task.isDaily)
        {
            GVar.dailyTaskComplete = true;
        }
        GVar.completedTasks.Add(task.id);
        AnalyticsManager.SendEvent(AnalyticsManager.EventType.Custom, AnalyticsManager.EventName.task_complete);
        btn.GetComponent<Animator>().SetTrigger("pop");
        StartCoroutine(DestroyLate(btn, 0.5f));
    }

    IEnumerator DestroyLate(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
        yield return null;
    }

    private void OnDisable()
    {
        GameData.Save(new SaveData());
    }

    public void BackButton()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
        SceneManager.LoadScene("Garden");
    }
}
