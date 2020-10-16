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

    [HideInInspector] public bool gameEnded = false;


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
            }
        }


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
        SceneManager.LoadScene("Garden");
    }
}
