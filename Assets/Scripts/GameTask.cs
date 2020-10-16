using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For now the tasks are just lists of flowers to collect

//TODO:
//  1. Create seperate lists for daily refreshable tasks and permament tasks

[CreateAssetMenu(fileName = "Task", order = 2, menuName = "ScriptableObjects/Task")]
public class GameTask : ScriptableObject
{
    public List<TaskRequirement> requirements;
    public int id;
    public int rewardXp;
}

[System.Serializable]
public class TaskRequirement
{
    public int flowerId;
    public int collectableAmount;
}
