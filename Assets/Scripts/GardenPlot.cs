using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GardenPlot : MonoBehaviour
{
    public bool hasFlower;
    public GameObject flower;
    public GameObject backgroundEffect;
    public ParticleSystem particle;
    public Text timerText;
    public TextMeshProUGUI countsText;
    public GameObject timerImage;

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

    public bool Fertilize()
    {
        if ((readyTime - System.DateTime.Now) > System.TimeSpan.Zero)
        {
            readyTime = System.DateTime.Now;
            GardenManager.grownPlants.Find(p => p.plotIndex == index).readyTime = System.DateTime.Now;
            return true;
        }
        return false;
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
        UpdateUI();
    }

    IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(1);
        onCooldown = false;
        flower.SetActive(false);
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
        CheckReadyStatus();
        UpdateValues();
        //Invoke("CheckReadyStatus", 2);
        UpdateUI();

    }

    void CheckReadyStatus()
    {
        System.TimeSpan timeToGrow = (readyTime - System.DateTime.Now);
        if (timeToGrow > System.TimeSpan.FromDays(30))
        {
            readyTime = System.DateTime.Now;
            timeToGrow = System.TimeSpan.Zero;
        }
        if (timeToGrow < System.TimeSpan.Zero)
        {
            flower.GetComponent<SpriteRenderer>().color = Color.white;
            harvestable = true;
            timerText.text = "";
            timerImage.SetActive(false);
        }
        else
        {
            flower.GetComponent<SpriteRenderer>().color = PlantDataBase.instance.unreadyColor;
            timerImage.SetActive(true);
        }
    }

    string AddZero(int num, int prev)
    {
        if (prev <= 0) { return num.ToString(); }
        return num < 10 ? "0" + num : num.ToString();
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
                timeText += timeToGrow.Minutes + timeToGrow.Hours > 0 ? AddZero(timeToGrow.Minutes, timeToGrow.Hours) + " : " : "";
                timeText += timeToGrow.Seconds + timeToGrow.Hours + timeToGrow.Minutes > 0 ? AddZero(timeToGrow.Seconds, timeToGrow.Hours + timeToGrow.Minutes) : "";
                timerText.text = timeText;
            }
        }
    }

    void UpdateUI()
    {
        string counts = "";
        int index = 0;

        foreach (int i in flowerCounts)
        {
            if(i > 0) counts += i + " <sprite=" + plant.flowers[index].index + ">\n";
            index++;
        }
        countsText.text = counts;
    }

    void HideTimer()
    {
        timerText.GetComponent<Animator>().SetBool("show", false);
        countsText.GetComponent<Animator>().SetBool("show", false);
        timerImage.SetActive(true);
    }

    void Harvest()
    {
        if (!harvestable)
        {
            timerText.GetComponent<Animator>().SetBool("show", true);
            countsText.GetComponent<Animator>().SetBool("show", true);
            timerImage.SetActive(false);
            Invoke("HideTimer", 3);
            return;
        }


        List<int> harvestableIndex = new List<int>();
        for (int i = 0; i < flowerCounts.Count; i++)
        {
            if (flowerCounts[i] > 0) harvestableIndex.Add(i);
        }

        if (harvestableIndex.Count < 1)
        {
            backgroundEffect.SetActive(true);
            GardenManager.instance.Harvest(5 + (int)(plant.value * 0.2f));
            GardenManager.instance.UpdateMoneyVisual();
            GardenManager.grownPlants.Remove(GardenManager.grownPlants.Find(p => p.plotIndex == index));
            flower.GetComponent<Animator>().SetTrigger("destroy");
            hasFlower = false;
            StartCoroutine(Cooldown());

        }
        else
        {
            SoundManager.instance.PlayHarvest();
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
            GardenManager.grownPlants.Find(p => p.plotIndex == index).flowerCounts = flowerCounts;
        }
        UpdateUI();
    }
}
