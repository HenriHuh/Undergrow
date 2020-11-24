using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public Sprite moneyIcon;
    public Sprite expansionIcon;
    public List<Item> items;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            GVar.moneyIcon = moneyIcon;
            GVar.expansionIcon = expansionIcon;

            //Check if multiple with same ID (Can delete mostly later)
            if (Application.isEditor)
            {
                List<int> ids = new List<int>();
                foreach (Item t in items)
                {
                    if (ids.Contains(t.index))
                    {
                        Debug.Log("Multiple items with same ID.");
                    }
                    ids.Add(t.index);
                }
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Item GetItemByIndex(int index)
    {
        return items.Find(i => i.index == index);
    }
    public Item GetItemByType(Item.Type type)
    {
        return items.Find(i => i.type == type);
    }

    public int GetIndexByType(Item.Type type)
    {
        return items.Find(i => i.type == type).index;
    }
}
