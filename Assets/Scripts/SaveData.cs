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

    //List of player owned seeds by index (e.g. [1, 1, 1, 2, 3, 5, 5])
    public List<int> seedsIndex = new List<int>();

    //List of player owned items
    public List<int> itemsIndex = new List<int>();

    //List of unlocked seeds that player can buy from the shop
    public List<int> unlockedSeedsIndex = new List<int>();

    //Total harvested flowers (index, count)
    public Dictionary<int, int> harvestedFlowers = new Dictionary<int, int>();

    public int collectiblesFound;

    //Completed levels
    public List<int> completedGoals = new List<int>();

    //Completed challenges by Index
    public List<int> completedTasks = new List<int>();

    //When the daily challenge will unlock next (previous challenge completion time + 20h?)
    public int dailyTaskTime;
    public int dailyTaskIndex;
    public bool dailyTaskComplete;

    public int gardenSize;

    public int experience;

    //How many level rewards the player has unlocked
    public int level;

    public bool newSave;

    //Quality settings as int (start at medium)
    public int quality = 1;

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
        itemsIndex = GVar.playerItemsIndex;
        unlockedSeedsIndex = GVar.unlockedSeedsIndex;

        harvestedFlowers = GVar.harvestedFlowers;
        collectiblesFound = GVar.collectiblesFound;
        experience = GVar.experience;
        gardenSize = GVar.gardenSize;
        launchCount = Tutorial.launchCount;

        completedGoals = GVar.completedGoals;
        completedTasks = GVar.completedTasks;
        version = GVar.saveVersion;
        newSave = GVar.newSave;

        dailyTaskTime = Tools.TimeToInt(GVar.dailyTaskTime);
        dailyTaskIndex = GVar.dailyTaskIndex;
        dailyTaskComplete = GVar.dailyTaskComplete;
        quality = (int)GVar.quality;
    }
}