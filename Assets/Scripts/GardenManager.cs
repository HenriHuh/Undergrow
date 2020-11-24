using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GardenManager : MonoBehaviour
{
    //Assign in editor
    public LayerMask flowerLayer;
    public LayerMask backLayer;
    public Transform flowerParent;
    public List<GameObject> activeMenus;
    public GameObject seedMenu;
    public GameObject settings;
    public Transform seedParent;
    public GameObject seedItemPrefab;
    public Image activeItem;
    public List<Transform> gardens;

    public Text moneyText;
    public Animator inventoryAnimator;
    public Text levelText;
    public GameObject tasksCompleteIndicator;

    public Text qualityText;
    public GameObject flowerPointer;
    public GameObject shopPointer;
    public GameObject taskPointer;
    public Slider musicSlider;
    public Slider effectSlider;

    //Static
    public static GardenManager instance;
    public static bool plantGrown = false;
    public bool hasFlower;
    public static bool shopOpenedTuturial;

    //Private
    //bool scrolling = false;
    Vector3 previousTouchPosition;
    bool holding = false;
    float holdTime;
    float scrollDirection;
    GardenPlot[] plots;
    bool flowerHarvested = false;
    bool sliderPressed = false;
    float sliderValue = 0;
    List<GardenPlot> plantedPlots = new List<GardenPlot>();
    Item selectedItem;

    //Static things
    public static List<PlantData> grownPlants = new List<PlantData>();
    public static PlantData currentPlant;
    public static int money = 40;
    //float maxScrollDist = 7.5f;
    static bool firstOpen = true;
    float tempMoney;
    int moneyVisual;
    Coroutine dragRoutine;
    float tickTimer = -1f;
    GameObject selectedPlotArea;
    //float waitTime = 0;
    public static bool tutorialStillOn;
    public static bool tutorialPart2StillOn;



    void OnApplicationQuit()
    {
        GameData.Save(new SaveData());
    }

    void Start()
    {
        instance = this;

        SetQuality(0);
        if (plantGrown)
        {
            if (GVar.plantedFlowers.ContainsKey(PlantDataBase.instance.GetIndexByName(currentPlant.plantName)))
            {
                GVar.plantedFlowers[PlantDataBase.instance.GetIndexByName(currentPlant.plantName)]++;
            }
            else
            {
                GVar.plantedFlowers.Add(PlantDataBase.instance.GetIndexByName(currentPlant.plantName), 1);
            }
            if (!GVar.completedTaskTypes.Contains(TaskRequirement.Type.anyPlant))
            {
                GVar.completedTaskTypes.Add(TaskRequirement.Type.anyPlant);
            }
            grownPlants.Add(currentPlant);
            SoundManager.instance.PlaySound(SoundManager.instance.sprout);
            AnalyticsManager.SendEvent(
                AnalyticsManager.EventType.Custom, 
                AnalyticsManager.EventName.plant_planted, 
                PlantDataBase.instance.GetPlantByName(currentPlant.plantName).index);

        }

        if (!firstOpen)
        {
            //Save game if a new plant has been grown
            GameData.Save(new SaveData());
        }
        firstOpen = false;

        if (Shop.gardenButton)
        {
            //GameData.Save(new SaveData());
        }

        SaveData data = GameData.saveData();

        money = data.money;
        tempMoney = money;
        moneyVisual = money;
        GVar.playerSeedsIndex = data.seedsIndex;
        GVar.playerItemsIndex = data.itemsIndex;
        GVar.experience = data.experience;
        GVar.gardenSize = data.gardenSize;
        GVar.completedGoals = data.completedGoals;
        GVar.completedTasks = data.completedTasks;
        GVar.unlockedSeedsIndex = data.unlockedSeedsIndex;
        grownPlants = PlantDataBase.instance.ConvertToData(data.plantData);
        moneyText.text = money.ToString();

        //Create layout
        //flowerParent.GetComponent<HorizontalObjectLayout>().UpdateLayout(3);
        //maxScrollDist = (flowerParent.GetComponent<HorizontalObjectLayout>().GetTotalWidth() / 2) - 4f;

        //Set current garden background by gardensize
        flowerParent = gardens[GVar.gardenSize > gardens.Count - 1 ? gardens.Count - 1 : GVar.gardenSize];
        foreach (Transform t in gardens)
        {
            t.gameObject.SetActive(false);
        }
        flowerParent.gameObject.SetActive(true);

        plots = flowerParent.GetComponentsInChildren<GardenPlot>();

        plots = plots.OrderBy(p => p.transform.position.x).ToArray();
        for (int i = 0; i < plots.Length; i++)
        {
            plots[i].index = i;
        }


        if (plantGrown)
        {
            //Vector3 focusPosition = Camera.main.transform.position;
            //focusPosition.x = plots[currentPlant.plotIndex].transform.position.x;
            //focusPosition.x = Mathf.Abs(focusPosition.x) > maxScrollDist ? maxScrollDist * Mathf.Sign(focusPosition.x) : focusPosition.x;
            //Camera.main.transform.position = focusPosition;
            //waitTime = -2f;
            SelectObject(GetNearestObject(Camera.main.transform.position, null), Camera.main.transform.position);
        }
        else
        {
            SelectObject(GetNearestObject(Camera.main.transform.position, null), Camera.main.transform.position);
            //Vector3 target = Camera.main.transform.position;
            //target.x = selectedPlotArea.transform.position.x;
            //Camera.main.transform.position = target;
        }
        plantGrown = false;



        foreach (PlantData p in grownPlants)
        {
            PlacePlant(p, p.plotIndex);
        }

        if (data.harvestedFlowers != null) GVar.harvestedFlowers = data.harvestedFlowers;

        if (GVar.playerSeedsIndex == null)
        {
            GVar.playerSeedsIndex = new List<int>();
            GVar.playerSeedsIndex.Add(3);
        }
        else if (!GVar.playerSeedsIndex.Contains(3))
        {
            GVar.playerSeedsIndex.Add(3);
        }

        if (TaskManager.instance.GetCompletedTasks().Count > 0)
        {
            tasksCompleteIndicator.GetComponent<Animator>().enabled = true;
            levelText.text = TaskManager.instance.GetCompletedTasks().Count.ToString();
        }
        else
        {
            levelText.text = "";
        }
    }

    void Update()
    {


        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, flowerLayer);
        if (Input.GetKeyUp(KeyCode.Mouse0) && !CheckActiveMenus() && hit.collider && !holding) //Hit a plot -> Select
        {
            if (activeItem.gameObject.activeSelf)
            {

                if (hit.transform.GetComponent<GardenPlot>().Fertilize())
                {
                    GVar.playerItemsIndex.Remove(selectedItem.index);
                }
                else
                {
                    activeItem.gameObject.SetActive(false);
                }
            }
            else
            {
                hit.transform.GetComponent<GardenPlot>().SelectPlot();
            }
            activeItem.gameObject.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.Mouse0) && !CheckActiveMenus()) //Hit background -> Start scrolling
        {
            //GetKeyDown

            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, backLayer);
            if (hit.collider)
            {

                previousTouchPosition = Input.mousePosition;
                //scrolling = true;
            }
        }
        if (Input.GetKey(KeyCode.Mouse0) && !CheckActiveMenus())
        {
            holdTime += Time.deltaTime;
            if (holdTime > 0.4f || Mathf.Abs(previousTouchPosition.x - Input.mousePosition.x) > 10)
            {
                holding = true;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Mouse0) && !seedMenu.activeSelf && waitTime >= 0)
        //{
        //    DragUI();
        //}

        ////TODO:
        ////SHOULD DELETE THIS CODE, BUT FOR SOME REASON THAT BREAKS FLOWER HARVESTING!!??

        ////Takes position between frames and translates picture in a shitty way
        //if (scrolling)
        //{
        //    //Have to change this at some point.. do I?
        //    scrollDirection = previousTouchPosition.x - Input.mousePosition.x;
        //    scrollDirection = Mathf.Abs(scrollDirection) > 200 ? 200 * Mathf.Sign(previousTouchPosition.x - Input.mousePosition.x) : scrollDirection;

        //    previousTouchPosition = Input.mousePosition;
        //}

        ////Move to direction and slow down over time
        //scrollDirection = Camera.main.transform.position.x + Mathf.Sign(scrollDirection) * 0.2f > maxScrollDist ? 0 : scrollDirection;
        //scrollDirection = Camera.main.transform.position.x + Mathf.Sign(scrollDirection) * 0.2f < -maxScrollDist ? 0 : scrollDirection;
        //scrollDirection = Mathf.MoveTowards(scrollDirection, 0, Time.deltaTime * 100);

        ////Make the sides of the screen bouncy
        //if (Camera.main.transform.position.x < -maxScrollDist)
        //{
        //    Vector3 targetpos = Camera.main.transform.position;
        //    targetpos.x = -maxScrollDist;
        //    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetpos, Time.deltaTime * 10);
        //}
        //else if (Camera.main.transform.position.x > maxScrollDist)
        //{
        //    Vector3 targetpos = Camera.main.transform.position;
        //    targetpos.x = maxScrollDist;
        //    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetpos, Time.deltaTime * 10);
        //}


        //if (waitTime < 0)
        //{
        //    waitTime += Time.deltaTime;
        //}
        //else
        //{
        //    Vector3 target = Camera.main.transform.position;
        //    target.x = selectedPlotArea.transform.position.x;
        //    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target, Time.deltaTime * 4);
        //}

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            //scrolling = false;
            holding = false;
            holdTime = 0;
        }

        tickTimer += Time.deltaTime;
        if (tickTimer > 1f)
        {
            tickTimer = 0;
            foreach (GardenPlot plot in plots)
            {
                plot.UpdateValues();
            }
        }

        if (sliderPressed && Input.GetKeyUp(KeyCode.Mouse0))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic, sliderValue);
            sliderPressed = false;
        }

        //Update money to make it fancier
        tempMoney = Mathf.Lerp(tempMoney, moneyVisual, Time.deltaTime * 7);
        moneyText.text = (Mathf.RoundToInt(tempMoney)).ToString();

        // Tutorial
        if (tutorialStillOn && flowerHarvested == false)
        {
            StartCoroutine(gardenTutorial());
        }

        if (tutorialStillOn && flowerHarvested)
        {
            StartCoroutine(gardenTutorial2());
        }

        if (tutorialPart2StillOn)
        {
            StartCoroutine(gardenTutorial3());
        }

        //Debug.Log(tutorialStillOn);
    }

    public bool CheckActiveMenus()
    {
        foreach (GameObject g in activeMenus)
        {
            if (g.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    public void CloseSettings()
    {
        Invoke("CloseSettingsInvokable", 1.5f);
    }

    void CloseSettingsInvokable()
    {
        settings.SetActive(false);
    }

    public void DragUI()
    {
        if (dragRoutine != null)
        {
            StopCoroutine(dragRoutine);
        }
        dragRoutine = StartCoroutine(StartDrag());
    }

    IEnumerator StartDrag()
    {
        Vector2 startPos = Input.mousePosition;

        while (Input.GetKey(KeyCode.Mouse0) && Mathf.Abs(startPos.x - Input.mousePosition.x) < Screen.width / 3)
        {
            yield return null;
        }
        if (Mathf.Abs(startPos.x - Input.mousePosition.x) > Screen.width / 4)
        {
            float dir = Mathf.Sign(startPos.x - Input.mousePosition.x);

            SelectObject(GetNearestObject(selectedPlotArea.transform.position + Vector3.right * dir, selectedPlotArea.transform), selectedPlotArea.transform.position + Vector3.right * dir);
        }

        //Start routine again if player keeps holding
        if (Input.GetKey(KeyCode.Mouse0))
        {
            dragRoutine = StartCoroutine(StartDrag());
        }

        yield return null;
    }

    public void ActivateItem(Item item)
    {
        selectedItem = item;
        switch (item.type)
        {
            case Item.Type.Fertilizer:
                activeItem.gameObject.SetActive(true);
                activeItem.sprite = item.icon;
                break;
            case Item.Type.SuperFertilizer:
                GVar.playerItemsIndex.Remove(item.index);
                foreach (GardenPlot g in plantedPlots)
                {
                    g.Fertilize();
                }
                break;
            default:
                break;
        }

    }

    GameObject GetNearestObject(Vector3 origin, Transform ignore)
    {
        GameObject nearest = flowerParent.GetChild(0).gameObject;
        float distance = Mathf.Infinity;
        foreach (Transform t in flowerParent)
        {
            if (Vector3.Distance(t.position, origin) < distance && t != ignore)
            {
                nearest = t.gameObject;
                distance = Vector3.Distance(t.position, origin);
            }
        }
        return nearest;

    }

    void SelectObject(GameObject obj, Vector3 origin)
    {
        if (selectedPlotArea != null && 
            Vector3.Distance(origin, obj.transform.position) 
            > Vector3.Distance(selectedPlotArea.transform.position, obj.transform.position))
        {
            return;
        }
        selectedPlotArea = obj;
    }


    //REMOVE THIS LATER
    int cheatcount = 0;
    public void Cheat()
    {
        GVar.playerItemsIndex.Add(ItemManager.instance.GetIndexByType(Item.Type.Chest));
        GVar.playerItemsIndex.Add(ItemManager.instance.GetIndexByType(Item.Type.Key));
        cheatcount++;
        foreach (GardenPlot plot in plots)
        {
            plot.Fertilize();
        }
        if (cheatcount > 4)
        {
            GVar.saveVersion++;
            GameData.Save(new SaveData());
        }
    }

    public void PopInventory()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.uiPop);
        inventoryAnimator.Play(0);
        moneyVisual = money;
    }

    public void UpdateMoneyVisual()
    {
        moneyVisual = money;
    }

    public void Harvest(int _money)
    {
        money += _money;
        if (TaskManager.instance.GetCompletedTasks().Count > 0)
        {
            tasksCompleteIndicator.GetComponent<Animator>().enabled = true;
            levelText.text = TaskManager.instance.GetCompletedTasks().Count.ToString();
        }
    }

    public void StartGame()
    {
        seedMenu.gameObject.SetActive(true);
    }

    void PlacePlant(PlantData plant, int index) //Place plants in correct positions
    {
        plots[index].plant = PlantDataBase.instance.GetPlantByName(plant.plantName).plantVariables;
        plots[index].GrowFlower(plant);
        plots[index].flowerCounts = plant.flowerCounts;
        plantedPlots.Add(plots[index]);
    }

    public void SelectSeed(string name)
    {

        //Checks the value of the plant.
        //Should be changed to be free later 
        //,since plant seeds are bought seperately from the shop.
        PlantVariables toSelect = new PlantVariables(PlantDataBase.instance.GetPlantByName(name).plantVariables);
        GVar.playerSeedsIndex.Remove(PlantDataBase.instance.GetIndexByName(name));

        //toSelect.plantVariables.plotIndex = GardenPlot.selected.index;
        GVar.currentPlant = toSelect;
        System.DateTime finishTime = System.DateTime.Now;

        //Set current plant (is used when garden manager gets loaded again)
        currentPlant = 
            new PlantData(toSelect.plantName, GardenPlot.selected.index, finishTime.AddMinutes(toSelect.timeToGrow), 
            new List<int>(new int[PlantDataBase.instance.GetPlantByName(toSelect.plantName).plantVariables.flowers.Count]));

        GardenPlot.selected.plant = toSelect;
        GVar.currentPlant.plotIndex = GardenPlot.selected.index;
        SceneManager.LoadScene("Tree");

    }

    public void SetQuality(int toChange) //0 = high
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Garden") return;
        
        int quality = (int)GVar.quality - toChange;
        quality = quality > 2 ? 0 : quality;
        quality = quality < 0 ? 2 : quality;
        switch (quality)
        {
            case 0:
                qualityText.text = "High";
                break;
            case 1:
                qualityText.text = "Medium";
                break;
            case 2:
                qualityText.text = "Low";
                break;
            default:
                break;
        }

        GVar.quality = (GVar.Quality)quality;
    }

    public void SetMusicVolume()
    {
        SoundManager.instance.SetMusicVolume(musicSlider.value);
        sliderValue = musicSlider.value;
        sliderPressed = true;
    }

    public void SetEffectVolume()
    {
        SoundManager.instance.SetEffectVolume(effectSlider.value);
        sliderValue = effectSlider.value;
        sliderPressed = true;
    }

    public void PlayClick()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
    }


    public void OpenTaskScreen()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
        SceneManager.LoadScene("Tasks");
    }

    IEnumerator gardenTutorial()
    {
        yield return new WaitForSeconds(1);
        flowerPointer.SetActive(true);

        flowerHarvested = true;

    }

    IEnumerator gardenTutorial2()
    {
        yield return new WaitForSeconds(2);
        flowerPointer.SetActive(false);

        yield return new WaitForSeconds(1);
        shopPointer.SetActive(true);
    }

    IEnumerator gardenTutorial3()
    {
        yield return new WaitForSeconds(1);
        taskPointer.SetActive(true);
    }

}
