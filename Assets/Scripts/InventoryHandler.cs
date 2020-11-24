using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public static InventoryHandler instance;

    public GameObject itemPrefab;
    public GameObject confirmScreen;
    public Button confirmBtn;
    public Transform itemParent;

    private void Start()
    {
        instance = this;
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        Dictionary<int, GameObject> createdItems = new Dictionary<int, GameObject>();

        foreach (Transform t in itemParent)
        {
            Destroy(t.gameObject);
        }

        foreach (int i in GVar.playerItemsIndex)
        {
            //TODO:
            //  Check if already added and increase count instead

            GameObject g;
            Item item = ItemManager.instance.GetItemByIndex(i);
            if (createdItems.ContainsKey(i))
            {
                g = createdItems[i];
                int count = 0;
                int.TryParse(g.transform.GetChild(3).GetComponent<Text>().text, out count);
                count = count < 1 ? 1 : count; 
                g.transform.GetChild(3).GetComponent<Text>().text = (count + 1).ToString();
            }
            else
            {
                g = Instantiate(itemPrefab, itemParent);
                createdItems.Add(i, g);
                g.transform.GetComponentInChildren<Text>().text = item.itemName;
                g.transform.GetChild(2).GetComponent<Image>().sprite = item.icon;
                g.GetComponent<Button>().onClick.AddListener(() => item.UseItem());
            }
        }
    }

    public void ActivateItem(Item item)
    {
        confirmScreen.SetActive(false);
        gameObject.SetActive(false);
        GardenManager.instance.ActivateItem(item);
    }

    public void AskItem(Item item)
    {
        confirmScreen.SetActive(true);
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(() => ActivateItem(item));
        //TODO:
        //  1. Open a menu to ask if player want's to use item
        //  2. Activate Item
    }

}
