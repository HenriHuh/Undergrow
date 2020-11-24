using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDataBase : MonoBehaviour
{
    public List<Plant> plants;
    public List<Flower> flowers; //These are not currently used for anything ??
    public Color unreadyColor;

    public static PlantDataBase instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (Application.isEditor)
        {
            //Just a backup because I'm definitely going to forget the names or IDs when making new plants
            List<string> names =  new List<string>();
            List<int> ids =  new List<int>();
            foreach (Plant p in plants)
            {
                if (names.Contains(p.plantVariables.plantName))
                {
                    Debug.LogError("Multiple plants with same name!");
                }
                if (ids.Contains(p.index))
                {
                    Debug.LogError("Multiple plants with same ID!");
                }
                names.Add(p.plantVariables.plantName);
                ids.Add(p.index);
            }
        }
    }


    public Plant GetPlantByName(string name)
    {
        return plants.Find(p => p.plantVariables.plantName == name);
    }
    public Plant GetPlantByIndex(int index)
    {
        return plants.Find(p => p.index == index);
    }

    public List<PlantVariables> GetPlantsByName(List<PlantData> datas)
    {
        List<PlantVariables> tempPlants = new List<PlantVariables>();
        foreach (PlantData data in datas)
        {
            PlantVariables temp = plants.Find(p => p.plantVariables.plantName == data.plantName).plantVariables;
            temp.plotIndex = data.plotIndex;
            tempPlants.Add(temp);
        }
        return tempPlants;
    }


    public string GetName(int index)
    {
        return plants.Find(p => p.index == index).plantVariables.plantName;
    }

    public static PlantData GetName(PlantVariables plant)
    {
        return new PlantData(plant.plantName, plant.plotIndex, System.DateTime.Now, new List<int>());
    }

    public static List<PlantData> GetNames(List<PlantVariables> plants)
    {
        List<PlantData> names = new List<PlantData>();
        foreach (PlantVariables plant in plants)
        {
            //PlantVariables temp = plants.Find(p => p.plantName == plant.plantName);
            names.Add(new PlantData(plant.plantName, plant.plotIndex, System.DateTime.Now, new List<int>()));
        }
        return names;
    }

    public int GetIndexByName(string _name)
    {
        return plants.Find(p => p.plantVariables.plantName == _name).index;
    }

    public List<List<int>> ConvertToList(List<PlantData> data)
    {
        List<List<int>> list = new List<List<int>>();
        foreach (PlantData d in data)
        {
            list.Add(new List<int>() {GetIndexByName(d.plantName), d.plotIndex, Tools.TimeToInt(d.readyTime), ConvertFlowerCountsToInt(d.flowerCounts) });
        }
        return list;
    }

    public List<PlantData> ConvertToData(List<List<int>> data)
    {
        List<PlantData> list = new List<PlantData>();
        foreach (List<int> d in data)
        {
            list.Add(new PlantData(GetName(d[0]), d[1], Tools.IntToTime(d[2]), ConvertToFlowerIndex(d[3])));
        }
        return list;
    }

    public List<int> ConvertToFlowerIndex(int flowerCounts)
    {
        List<int> flowers = new List<int>();
        for (int i = 0; i < flowerCounts.ToString().Length; i++)
        {
            flowers.Add(int.Parse(flowerCounts.ToString()[i].ToString())); //UGH
        }
        return flowers;
    }

    public int ConvertFlowerCountsToInt(List<int> flowers)
    {
        string flowerString = "";
        foreach (int f in flowers)
        {
            flowerString += f < 1 ? "0" : f.ToString();
        }
        return int.Parse(flowerString);
    }



    //Flowers

    public string GetFlowerName(int index)
    {
        return flowers.Find(f => f.index == index).flowerName;
    }


}
