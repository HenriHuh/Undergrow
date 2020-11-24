using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class GameData
{
    public static void Save(SaveData saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();


        string path = Application.persistentDataPath + "/garden.save"; // "garden.save" -> "/garden.save"

        FileStream file = new FileStream(path, FileMode.Create);

        formatter.Serialize(file, saveData);

        file.Close();


    }

    public static void SaveBackUp(SaveData saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();
        string path = Application.persistentDataPath + "/gardenbackup.save";
        FileStream file = new FileStream(path, FileMode.Create);
        formatter.Serialize(file, saveData);
        file.Close();
    }

    public static bool CheckExists()
    {
        string path = Application.persistentDataPath + "/garden.save";
        return File.Exists(path);

    }

    public static SaveData saveData()
    {
        string path = Application.persistentDataPath + "/garden.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            if (GVar.saveVersion != data.version)
            {
                Debug.Log("Old save version!");
                GVar.newSave = true;
                Save(data);
                return new SaveData();
            }

            return data;
        }
        else
        {
            Debug.Log("Save file not found, try again");
            GVar.newSave = true;
            return new SaveData();
        }
    }
    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }

}