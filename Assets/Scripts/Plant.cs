using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Plant", order = 1,menuName ="ScriptableObjects/Plant")]
public class Plant : ScriptableObject
{
    public PlantVariables plantVariables;
    public int index;
}

[System.Serializable]
public class PlantVariables
{
    public PlantVariables(PlantVariables _plant)
    {
        plantName =     _plant.plantName;
        value =         _plant.value;
        depth =         _plant.depth;
        speed =         _plant.speed;
        flowerAnim =    _plant.flowerAnim;
        flowers =       _plant.flowers;
        mapObjects =    _plant.mapObjects;
        plotIndex =     _plant.plotIndex;
        timeToGrow =    _plant.timeToGrow;
        silhouette =    _plant.silhouette;
        flowerSprite =        _plant.flowerSprite;
    }

    public string plantName;
    public int value;
    public float depth;
    public float speed;
    public Sprite seedSprite;
    public Sprite flowerSprite;
    public RuntimeAnimatorController flowerAnim;
    public Sprite silhouette;
    [Tooltip("Time for plant to grow in minutes.")] public int timeToGrow;

    public List<Flower> flowers;
    public List<MapObject> mapObjects;

    [HideInInspector] public int plotIndex; //Index where plant was planted in GardenManager
}

[System.Serializable]
public class PlantData
{
    public PlantData(string _plantname, int _plotIndex, System.DateTime _readyTime, List<int> _flowerCounts)
    {
        plantName = _plantname;
        plotIndex = _plotIndex;
        readyTime = _readyTime;
        flowerCounts = _flowerCounts;
    }
    public List<int> flowerCounts;
    public string plantName;
    public int plotIndex;
    public System.DateTime readyTime;
}
