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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Check if multiple with same ID (Can delete mostly later)
            List<int> ids = new List<int>();
            foreach (GameTask t in dailyTasks)
            {
                if (ids.Contains(t.id))
                {
                    Debug.Log("Multiple tasks with same ID.");
                }
                ids.Add(t.id);
                allTasks.Add(t); //<- Don't delete this
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
            if (!GVar.harvestedFlowers.ContainsKey(tr.flowerId) || GVar.harvestedFlowers[tr.flowerId] < tr.collectableAmount)
            {
                return false;
            }
        }
        return true;
    }
}
