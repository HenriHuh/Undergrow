using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


//Handles UI elements in the root game
public class UIManager : MonoBehaviour
{
    [Tooltip("Image prefab.")] public Image flowerImage;
    public Transform flowerParent;
    public static UIManager instance;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject itemScreen;

    Coroutine hideItems;

    void Start()
    {
        instance = this;

        InstantiateFlowers();
        hideItems = StartCoroutine(HideItemScreen());
    }

    public void InstantiateFlowers()
    {
        for (int i = flowerParent.childCount; i < GVar.currentPlant.flowers.Count; i++)
        {
            Instantiate(flowerImage, flowerParent);
            flowerParent.GetChild(i).GetComponent<Image>().sprite = GVar.currentPlant.flowers[i].icons[0];
        }
    }

    public void UpdateFlowers(List<int> flowers)
    {
        for (int i = 0; i < flowers.Count; i++)
        {
            flowerParent.GetChild(i).GetComponent<Image>().sprite = GVar.currentPlant.flowers[i].icons[flowers[i]];
        }

        if (hideItems != null)
        {
            StopCoroutine(hideItems);
        }
        hideItems = StartCoroutine(HideItemScreen());
    }

    IEnumerator HideItemScreen()
    {
        itemScreen.GetComponent<Animator>().SetBool("Show", true);
        yield return new WaitForSeconds(2);
        itemScreen.GetComponent<Animator>().SetBool("Show", false);
        yield return null;
    }

    public void WinScreen()
    {
        winScreen.SetActive(true);
    }

    public void LoseScreen()
    {
        loseScreen.SetActive(true);
    }

}
