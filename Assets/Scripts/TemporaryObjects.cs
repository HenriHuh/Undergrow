using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryObjects : MonoBehaviour
{
    //Add game objects here so they will eventually be destroyed
    //to prevent memory flooding too much

    public static TemporaryObjects instance;
    int maxObjCount = 70;
    List<GameObject> objects = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    public void AddItem(GameObject obj)
    {

        objects.Add(obj);
        if (objects.Count > maxObjCount)
        {
            Destroy(objects[0]);
            objects.RemoveAt(0);
        }
    }
}
