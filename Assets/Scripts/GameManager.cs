using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float gameTime;
    public List<int> collectedFlowers = new List<int>();
    public SpriteRenderer flowerSilhouette;
    public LayerMask obstacleLayer;

    [HideInInspector] public List<int> totalCollectedFlowers = new List<int>(); //For Endless mode
    [HideInInspector] public bool gameEnded = false;
    public static bool isEndless = false;


    void Start()
    {
        flowerSilhouette.sprite = GVar.currentPlant.silhouette;
        gameTime = 0;
        instance = this;
    }


    public void CollectItem(int id) //This is for collecting flowers
    {
        //Instantiate list if first pick up
        if (collectedFlowers.Count == 0)
        {
            for (int i = 0; i < GVar.currentPlant.flowers.Count; i++)
            {
                collectedFlowers.Add(0);
                totalCollectedFlowers.Add(0);
            }
        }


        if (isEndless)
        {
            bool isFull = true;
            for (int i = 0; i < collectedFlowers.Count; i++)
            {
                if (collectedFlowers[i] + 1 < GVar.currentPlant.flowers[i].icons.Count)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                int i = 0;
                foreach (int count in collectedFlowers)
                {
                    if (GVar.harvestedFlowers.ContainsKey(GVar.currentPlant.flowers[i].index))
                    {
                        GVar.harvestedFlowers[GVar.currentPlant.flowers[i].index] += count;
                    }
                    else
                    {
                        GVar.harvestedFlowers.Add(GVar.currentPlant.flowers[i].index, count);
                    }
                    i++;
                }

                //Clear flowers
                for (int index = 0; index < collectedFlowers.Count; index++)
                {
                    collectedFlowers[index] = 0;
                }
                SoundManager.instance.PlaySound(SoundManager.instance.rootFinish);
            }
        }

        totalCollectedFlowers[id] = collectedFlowers[id] + 1 < GVar.currentPlant.flowers[id].icons.Count ? totalCollectedFlowers[id] + 1 : totalCollectedFlowers[id];
        collectedFlowers[id] = collectedFlowers[id] + 1 < GVar.currentPlant.flowers[id].icons.Count ? collectedFlowers[id] + 1 : collectedFlowers[id];
        TreeVisualizer.instance.AddFlower(GVar.currentPlant.flowers[id].flower);
        //WISH: Maybe some extra points for collecting more than maximum amount of flowers

        UIManager.instance.UpdateFlowers(collectedFlowers);
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    public void LoadGardenScene()
    {
        SoundManager.instance.PlayMusic(SoundManager.instance.musicGarden);
        if (isEndless)
        {
            int i = 0;
            foreach (int count in collectedFlowers)
            {
                if (GVar.harvestedFlowers.ContainsKey(GVar.currentPlant.flowers[i].index))
                {
                    GVar.harvestedFlowers[GVar.currentPlant.flowers[i].index] += count;
                }
                else
                {
                    GVar.harvestedFlowers.Add(GVar.currentPlant.flowers[i].index, count);
                }
                i++;
            }
            GameData.Save(new SaveData());
            SceneManager.LoadScene("StartScreen");
        }
        else
        {
            SceneManager.LoadScene("Garden");
        }
    }
}
