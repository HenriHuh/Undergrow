using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;
    public List<GameTask> basicTasks;
    public List<GameTask> dailyTasks;

    //A list of all open tasks for the player
    [HideInInspector] public List<GameTask> allTasks = new List<GameTask>();

    void Awake()
    {
        int xpt = 0;
        foreach (GameTask t in basicTasks)
        {
            xpt += t.rewardXp;
        }
        Debug.Log("total: " + xpt);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Check if should make new daily task 
            SaveData data = GameData.saveData();
            if (System.DateTime.Now > Tools.IntToTime(data.dailyTaskTime))
            {
                GVar.dailyTaskTime = System.DateTime.Now;
                GVar.dailyTaskTime = GVar.dailyTaskTime.AddHours(20);
                GVar.dailyTaskIndex = dailyTasks[Random.Range(0, dailyTasks.Count)].id;
                GVar.dailyTaskComplete = false;
                GVar.collectiblesFound = 0;
            }
            else
            {
                GVar.dailyTaskTime = Tools.IntToTime(data.dailyTaskTime);
                GVar.dailyTaskIndex = data.dailyTaskIndex;
                GVar.dailyTaskComplete = data.dailyTaskComplete;
                GVar.collectiblesFound = data.collectiblesFound;
            }

            //Check if multiple with same ID (Can delete mostly later)
            List<int> ids = new List<int>();
            foreach (GameTask t in dailyTasks)
            {
                if (ids.Contains(t.id))
                {
                    Debug.Log("Multiple tasks with same ID.");
                }
                ids.Add(t.id);
            }

            if (!GVar.dailyTaskComplete)
            {
                allTasks.Add(dailyTasks.Find(t => t.id == GVar.dailyTaskIndex));
            }
            foreach (GameTask t in basicTasks)
            {
                if (ids.Contains(t.id))
                {
                    Debug.Log("Multiple tasks with same ID.");
                }
                ids.Add(t.id);
                allTasks.Add(t); //<- Don't delete this
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public List<GameTask> GetCompletedTasks()
    {
        List<GameTask> toList = new List<GameTask>();
        foreach (GameTask t in allTasks)
        {
            if (GVar.completedTasks.Contains(t.id))
            {
                continue;
            }
            else if (CheckCompleted(t))
            {
                toList.Add(t);
            }
        }
        return toList;
    }

    /// <summary>
    /// Get tasks ordered from completed to not completed
    /// </summary>
    public List<GameTask> GetOrderedTasks()
    {
        List<GameTask> toList = new List<GameTask>();
        foreach (GameTask t in allTasks)
        {
            if (GVar.completedTasks.Contains(t.id))
            {
                continue;
            }
            else if (!CheckCompleted(t))
            {
                toList.Add(t);
            }
            else
            {
                toList.Insert(0, t);
            }
        }
        return toList;
    }

    bool CheckCompleted(GameTask t)
    {
        foreach (TaskRequirement tr in t.requirements)
        {
            if (tr.type == TaskRequirement.Type.flower && (!GVar.harvestedFlowers.ContainsKey(tr.reqId) || GVar.harvestedFlowers[tr.reqId] < tr.collectableAmount))
            {
                return false;
            }
            else if (tr.type == TaskRequirement.Type.plant && (!GVar.plantedFlowers.ContainsKey(tr.reqId) || GVar.plantedFlowers[tr.reqId] < tr.collectableAmount))
            {
                return false;
            }
            else if (tr.type != TaskRequirement.Type.flower && tr.type != TaskRequirement.Type.plant && !GVar.completedTaskTypes.Contains(tr.type))
            {
                return false;
            }
        }
        return true;
    }
}
