using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GardenPlot : MonoBehaviour
{
    public bool hasFlower;
    public GameObject flower;
    public GameObject backgroundEffect;
    public ParticleSystem particle;
    public Text timerText;

    [HideInInspector] public PlantVariables plant;
    [HideInInspector] public int index;
    [HideInInspector] public List<int> flowerCounts = new List<int>();
    public static GardenPlot selected;

    private bool onCooldown;
    bool harvestable = false;

    [HideInInspector] public System.DateTime readyTime;

    private void Start()
    {
        //Just a backup so the save system doesn't fuck up
        if (selected == null) selected = this;
    }

    public void SelectPlot()
    {
        if (onCooldown) return;

        if (!hasFlower)
        {
            GardenManager.instance.StartGame();
        }
        else
        {
            Harvest();
        }
        selected = this;
        plant.plotIndex = index;
    }

    IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(1);
        onCooldown = false;
        yield return null;
    }

    public void GrowFlower(PlantData data)
    {
        hasFlower = true;
        backgroundEffect.SetActive(false);
        flower.SetActive(true);
        readyTime = data.readyTime;
        flower.GetComponent<SpriteRenderer>().sprite = plant.flowerSprite;
        flower.GetComponent<Animator>().runtimeAnimatorController = plant.flowerAnim;
        Invoke("CheckReadyStatus", 2);
    }

    void CheckReadyStatus()
    {
        System.TimeSpan timeToGrow = (readyTime - System.DateTime.Now);
        if (timeToGrow < System.TimeSpan.Zero)
        {
            flower.GetComponent<SpriteRenderer>().color = Color.white;
            harvestable = true;
            timerText.text = "";
        }
        else
        {
            flower.GetComponent<SpriteRenderer>().color = Color.gray;

        }
    }

    public void UpdateValues()
    {
        if (hasFlower)
        {
            System.TimeSpan timeToGrow = (readyTime - System.DateTime.Now);
            if (timeToGrow < System.TimeSpan.Zero)
            {
                CheckReadyStatus();
            }
            else
            {
                string timeText = "";
                timeText += timeToGrow.Hours > 0 ? timeToGrow.Hours + " : " : "";
                timeText += timeToGrow.Minutes > 0 ? timeToGrow.Minutes + " : " : "";
                timeText += timeToGrow.Seconds > 0 ? timeToGrow.Seconds.ToString() : "";
                timerText.text = timeText;
            }
        }
    }

    void Harvest()
    {
        if (!harvestable) return;

        List<int> harvestableIndex = new List<int>();
        for (int i = 0; i < flowerCounts.Count; i++)
        {
            if (flowerCounts[i] > 0) harvestableIndex.Add(i);
        }

        if (harvestableIndex.Count < 1)
        {
            backgroundEffect.SetActive(true);
            GardenManager.instance.Harvest(5 + (int)(plant.value * 0.5f));
            GardenManager.instance.UpdateMoneyVisual();
            GardenManager.grownPlants.Remove(GardenManager.grownPlants.Find(p => p.plantName == plant.plantName));
            flower.SetActive(false);
            hasFlower = false;
            StartCoroutine(Cooldown());
        }
        else
        {
            SoundManager.instance.PlaySound(SoundManager.instance.harvest);
            int rand = Random.Range(0, harvestableIndex.Count);
            int harvest = harvestableIndex[rand];
            flowerCounts[harvest]--;
            GardenManager.instance.Harvest(plant.flowers[rand].value);
            flower.GetComponent<Animator>().SetTrigger("harvest");

            if (GVar.harvestedFlowers.ContainsKey(plant.flowers[harvest].index))
            {
                GVar.harvestedFlowers[plant.flowers[harvest].index]++;
            }
            else
            {
                GVar.harvestedFlowers.Add(plant.flowers[harvest].index, 1);
            }

            Material mat = plant.flowers[harvest].material;
            particle.GetComponent<Renderer>().material = mat;
            particle.transform.GetChild(0).GetComponent<Renderer>().material = mat;
            particle.Play();
            Destroy(particle, 4);
            particle = Instantiate(particle, particle.transform.parent);
        }
    }
}
