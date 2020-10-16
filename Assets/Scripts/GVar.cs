using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static class for storing global variables

public static class GVar
{

    //Current plant being grown
    public static PlantVariables currentPlant;

    //Garden size used in garden scene
    public static int gardenSize = 1;

    //Seeds owned by the player listed by their index
    public static List<int> playerSeedsIndex =  new List<int>();

    //Total harvested flowers by index
    public static Dictionary<int, int> harvestedFlowers = new Dictionary<int, int>();

    //Completed levels
    public static List<int> completedGoals = new List<int>();

    //Completed challenges by their ID
    public static List<int> completedTasks = new List<int>();

    //Total collectibles found
    public static int collectiblesFound;

    //Experience
    public static int experience;

    //Version number save system so if the game gets updated everything doesn't explode
    public static int saveVersion = 4;


    //Constant values

    //Radius of damage when player collides with a destroyable object
    public const int damageRadius = 2;

}
