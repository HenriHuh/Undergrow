using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<LevelGoal> levels;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Get index in the complete level list
    /// </summary>
    public int GetIndex(LevelGoal goal)
    {
        return levels.IndexOf(goal);
    }
}
