using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //Version number to check if game has been updated
    public int version;

    public int money = 0;

    //Plant data is stored as integer lists (id, plotIndex, plantTime, flowerCounts)
    public List<List<int>> plantData = new List<List<int>>();

    //List of player owned seeds (e.g. [1, 1, 1, 2, 3, 5, 5])
    public List<int> seedsIndex = new List<int>();

    //Total harvested flowers (index, count)
    public Dictionary<int, int> harvestedFlowers = new Dictionary<int, int>();

    public int collectiblesFound;

    //Completed levels
    public List<int> completedGoals = new List<int>();

    //Completed challenges by Index
    public List<int> completedTasks = new List<int>();

    //When the daily challenge will unlock next (previous challenge completion time + 20h?)
    public int dailyChallengeTime;

    public int gardenSize;

    public int experience;

    //How many level rewards the player has unlocked
    public int level;

    //How many times the game is opened
    public int launchCount;


    //GameObjects and ScriptableObjects can not be serialized.
    //Serializable classes cannot be saved as a list as the values
    //will be the same for all of them.

    public SaveData ()
    {
        money = GardenManager.money;
        plantData = PlantDataBase.instance.ConvertToList(GardenManager.grownPlants);
        seedsIndex = GVar.playerSeedsIndex;

        harvestedFlowers = GVar.harvestedFlowers;
        collectiblesFound = GVar.collectiblesFound;
        experience = GVar.experience;
        gardenSize = GVar.gardenSize;
        launchCount = Tutorial.launchCount;

        completedGoals = GVar.completedGoals;
        completedTasks = GVar.completedTasks;
        version = GVar.saveVersion;
    }
}