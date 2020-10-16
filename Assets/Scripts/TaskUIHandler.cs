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

    public Sprite enabledButtonSprite;
    public Sprite disabledButtonSprite;


    float tempExp;
    List<GameObject> goalObjects = new List<GameObject>();
    List<LevelGoal> levelGoals = new List<LevelGoal>();
    GameObject activeObject;
    LevelGoal activeGoal;
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
            content.sizeDelta = content.sizeDelta + g.GetComponent<RectTransform>().sizeDelta * Vector2.up * 1.075f;
            g.GetComponent<Button>().onClick.AddListener(() => SelectTask(t, g));

            string requires = "";
            foreach (TaskRequirement req in t.requirements)
            {
                int count = 0;
                if (GVar.harvestedFlowers.ContainsKey(req.flowerId))
                {
                    count = GVar.harvestedFlowers[req.flowerId];
                }

                requires += count + "/" + req.collectableAmount + " <sprite=" + req.flowerId + ">";
            }

            g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = requires;
            g.transform.GetChild(2).GetComponent<Text>().text = t.rewardXp + "xp";
            foreach (TaskRequirement tr in t.requirements)
            {
                if (!GVar.harvestedFlowers.ContainsKey(tr.flowerId) || GVar.harvestedFlowers[tr.flowerId] < tr.collectableAmount)
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = disabledButtonSprite;
                    break;
                }

            }
        }

        foreach (LevelGoal lvl in LevelManager.instance.levels)
        {
            GameObject g = Instantiate(levelIconPrefab, levelParent);
            g.GetComponent<Image>().sprite = lvl.rewardIcon;
            g.GetComponentInChildren<Text>().text = lvl.rewardCount > 1 ? lvl.rewardCount.ToString() : "";
            goalObjects.Add(g);
            levelGoals.Add(lvl);
            if (GVar.experience >= lvl.requiredExp)
            {
                prevGoal = lvl;
                g.transform.GetChild(2).GetComponent<Image>().fillAmount = 1;

                if (!GVar.completedGoals.Contains(LevelManager.instance.GetIndex(lvl)))
                {
                    GVar.completedGoals.Add(LevelManager.instance.GetIndex(lvl));
                    lvl.GetReward();
                    g.GetComponent<Animator>().SetTrigger("pop");
                }
                else
                {
                    Destroy(g.GetComponent<Animator>());
                    g.GetComponent<Image>().color = Color.clear;
                    g.GetComponentInChildren<Text>().text = "";
                }
            }
            else if (activeGoal == null)
            {
                activeObject = g;
                activeGoal = lvl;
                g.transform.GetChild(2).GetComponent<Image>().fillAmount = prevGoal == null 
                    ? (float)GVar.experience / activeGoal.requiredExp : (float)(GVar.experience - prevGoal.requiredExp) / activeGoal.requiredExp; 
            }
        }

        //All levels completed
        if (activeGoal == null)
        {
            completed = true;
            activeGoal = levelGoals[levelGoals.Count - 1];
        }

        levelParentOrigin = levelParent.transform.position;
        UpdateUI();
    }


    void Update()
    {
        levelParent.transform.position = Vector3.Lerp(levelParent.transform.position, levelParentTargetPos, Time.deltaTime * 5);

        if (completed) return;

        float target = prevGoal == null
            ? (float)GVar.experience / activeGoal.requiredExp : (float)(GVar.experience - prevGoal.requiredExp) / (activeGoal.requiredExp - prevGoal.requiredExp);
        target = target > activeGoal.requiredExp ? activeGoal.requiredExp : target;

        tempExp = Mathf.Lerp(tempExp, target, Time.deltaTime * 2);
        activeObject.transform.GetChild(2).GetComponent<Image>().fillAmount = tempExp;


        if (GVar.experience >= activeGoal.requiredExp && activeObject.transform.GetChild(2).GetComponent<Image>().fillAmount > 0.95f)
        {
            NextGoal();
            UpdateUI();
        }
    }


    bool CheckCompleted(GameTask t)
    {
        foreach (TaskRequirement tr in t.requirements)
        {
            if (!GVar.harvestedFlowers.ContainsKey(tr.flowerId) || GVar.harvestedFlowers[tr.flowerId] < tr.collectableAmount)
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
            activeObject.GetComponent<Animator>().SetTrigger("pop");
            GVar.completedGoals.Add(LevelManager.instance.GetIndex(activeGoal));
            SoundManager.instance.PlaySound(SoundManager.instance.completeLevel);
        }

        int index = levelGoals.IndexOf(activeGoal) + 1;
        if (index < levelGoals.Count)
        {
            activeGoal = levelGoals[index];
            activeObject = goalObjects[index];
        }
        else
        {
            completed = true;
        }
        tempExp = 0;

    }

    void UpdateUI()
    {
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
            if (!GVar.harvestedFlowers.ContainsKey(tr.flowerId) || GVar.harvestedFlowers[tr.flowerId] < tr.collectableAmount)
            {
                return;
            }

        }
        SoundManager.instance.PlaySound(SoundManager.instance.completeTask);
        GVar.experience += task.rewardXp;
        GVar.completedTasks.Add(task.id);
        content.sizeDelta = content.sizeDelta - btn.GetComponent<RectTransform>().sizeDelta * Vector2.up * 1.075f;
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
