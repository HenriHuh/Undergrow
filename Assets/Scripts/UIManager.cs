using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Handles UI elements in the root game
public class UIManager : MonoBehaviour
{
    [Tooltip("Image prefab.")] public Image flowerImage;
    public Image endlessFlowerImg;
    public Transform flowerParent;
    public static UIManager instance;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject endlessScreen;
    public GameObject itemScreen;
    public List<GameObject> disableOnEndless;
    public Text itemInvincibilityTxt, ItemWaterTxt;
    Coroutine hideItems;
    public GameObject pauseScreen;

    bool paused = false;

    void Start()
    {
        instance = this;

        InstantiateFlowers();
        hideItems = StartCoroutine(HideItemScreen());

        if (MapGen.instance.isEndless)
        {
            foreach (GameObject g in disableOnEndless)
            {
                g.SetActive(false);
            }

            Vector2 size = flowerParent.GetComponent<RectTransform>().sizeDelta;
            size.y *= 1.5f;
            flowerParent.GetComponent<RectTransform>().sizeDelta = size;
        }
        UpdateItems();
    }

    public void InstantiateFlowers()
    {
        //Create setup for collectibles panel
        //First and last element of panel layout are the borders
        for (int i = flowerParent.childCount; i < GVar.currentPlant.flowers.Count + 2; i++)
        {
            Image img = Instantiate(flowerImage, flowerParent);
            img.transform.SetSiblingIndex(i - 1);
            flowerParent.GetChild(i - 1).GetComponent<Image>().sprite = GVar.currentPlant.flowers[i-2].icons[0];
        }
    }

    public void PauseGame()
    {
        paused = !paused;
        if (paused)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0;
            SoundManager.instance.music.volume /= 3;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
            SoundManager.instance.music.volume *= 3;
        }
    }

    public void UpdateFlowers(List<int> flowers)
    {
        for (int i = 0; i < flowers.Count; i++)
        {
            flowerParent.GetChild(i + 1).GetComponent<Image>().sprite = GVar.currentPlant.flowers[i].icons[flowers[i]];
        }

        if (hideItems != null)
        {
            StopCoroutine(hideItems);
        }
        hideItems = StartCoroutine(HideItemScreen());
    }

    public void UpdateItems()
    {
        int waterCount = 0;
        int invCount = 0;

        foreach (int i in GVar.playerItemsIndex)
        {
            if (i == 7)
            {
                waterCount++;
            }
            else if (i == 8)
            {
                invCount++;
            }
        }

        if (waterCount < 1)
        {
            ItemWaterTxt.GetComponentInParent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
        if (invCount < 1)
        {
            itemInvincibilityTxt.GetComponentInParent<Image>().color = new Color(1, 1, 1, 0.5f);
        }


        itemInvincibilityTxt.text = invCount.ToString();
        ItemWaterTxt.text = waterCount.ToString();
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

        //Move collectibles to endscreen
        foreach (Transform t in flowerParent)
        {
            GameObject g = Instantiate(t.gameObject, winScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform);
            g.transform.localScale /= 2;
        }

        //Destroy the borders from layout
        Destroy(winScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.GetChild(0).gameObject);
        Destroy(winScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.GetChild(
            winScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.childCount - 1).gameObject);

        //Kill the moles!!
        for (int i = SimpleEnemy.moles.Count - 1; i >= 0; i--)
        {
            Destroy(SimpleEnemy.moles[i]);
        }
    }

    public void LoseScreen()
    {
        if (GameManager.isEndless)
        {
            endlessScreen.SetActive(true);


            for (int i = 0; i < GameManager.instance.totalCollectedFlowers.Count; i++)
            {
                Image img = Instantiate(endlessFlowerImg, endlessScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform);
                endlessScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.GetChild(i).GetComponent<Image>().sprite = 
                    GVar.currentPlant.flowers[i].icons[GVar.currentPlant.flowers[i].icons.Count - 1];
                img.transform.GetChild(0).GetComponent<Text>().text = GameManager.instance.totalCollectedFlowers[i].ToString();
            }
        }
        else
        {
            loseScreen.SetActive(true);

            //Move collectibles to endscreen
            foreach (Transform t in flowerParent)
            {
                GameObject g = Instantiate(t.gameObject, loseScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform);
                g.transform.localScale /= 2;
            }

            //Destroy the borders from layout
            Destroy(loseScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.GetChild(0).gameObject);
            Destroy(loseScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.GetChild(
                loseScreen.GetComponentInChildren<HorizontalLayoutGroup>().transform.childCount - 1).gameObject);
        }

        //Kill the moles!!
        for (int i = SimpleEnemy.moles.Count - 1; i >= 0; i--)
        {
            Destroy(SimpleEnemy.moles[i]);
        }
    }

}
