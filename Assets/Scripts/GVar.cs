using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static class for storing global variables

public static class GVar
{

    //Current plant being grown
    public static PlantVariables currentPlant;

    //Collectable flowers ids for root game. When collected all flowers -> remove index
    public static List<int> collectableFlowerIndex = new List<int>();

    //Garden size used in garden scene
    public static int gardenSize = 0;

    //Seeds owned by the player listed by their index
    public static List<int> playerSeedsIndex =  new List<int>();

    //Items owned by the player listed by their index
    public static List<int> playerItemsIndex = new List<int>();

    //Total harvested flowers by index
    public static Dictionary<int, int> harvestedFlowers = new Dictionary<int, int>();

    //Completed levels
    public static List<int> completedGoals = new List<int>();

    //Completed challenges by their ID
    public static List<int> completedTasks = new List<int>();

    //List of daily tasks completed
    public static List<TaskRequirement.Type> completedTaskTypes = new List<TaskRequirement.Type>();

    //Planted flowers by index
    public static Dictionary<int, int> plantedFlowers = new Dictionary<int, int>();

    //List of unlocked seeds that player can buy from the shop
    public static List<int> unlockedSeedsIndex = new List<int>() { 0, 1, 2 };

    //Total collectibles found
    public static int collectiblesFound;

    //Experience
    public static int experience;

    //Version number save system so if the game gets updated everything doesn't explode
    public static int saveVersion = 13;

    //When a new daily task will unlock
    public static System.DateTime dailyTaskTime;

    //Current daily task id
    public static int dailyTaskIndex;

    //Items used by player in session
    public static int itemsUsed;

    //root game started
    public static int seedPlanted;

    //root game finished without failing
    public static int seedFinished;

    public static bool dailyTaskComplete = false;

    public static bool newSave = false;

    public enum Quality {high, medium, low}
    public static Quality quality = Quality.medium;


    public static Sprite moneyIcon;
    public static Sprite expansionIcon;

    //Constant values

    //Radius of damage when player collides with a destroyable object
    public const int damageRadius = 2;

    //How much faster water drains when going through poison
    public const int waterPoisonedMultiplier = 6;

}
