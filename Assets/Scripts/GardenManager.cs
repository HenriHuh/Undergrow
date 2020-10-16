using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Linq;

public class GardenManager : MonoBehaviour
{
    //Assign in editor
    public LayerMask flowerLayer;
    public LayerMask backLayer;
    public Transform flowerParent;
    public GameObject seedMenu;
    public Transform seedParent;
    public GameObject seedItemPrefab;

    public Text moneyText;
    public Animator inventoryAnimator;
    public Text levelText;
    public GameObject tasksCompleteIndicator;

    public GameObject flowerPointer;
    public GameObject shopPointer;

    //Static
    public static GardenManager instance;
    public static bool plantGrown = false;
    public bool hasFlower;

    //Private
    bool scrolling = false;
    Vector3 previousTouchPosition;
    bool holding = false;
    float holdTime;
    float scrollDirection;
    GardenPlot[] plots;

    //Static things
    public static List<PlantData> grownPlants = new List<PlantData>();
    public static PlantData currentPlant;
    public static int money = 40;
    float maxScrollDist = 7.5f;
    static bool firstOpen = true;
    float tempMoney;
    int moneyVisual;
    Coroutine dragRoutine;
    float tickTimer = -1f;
    GameObject selectedPlotArea;
    float waitTime = 0;
    public static bool tutorialStillOn;

    void OnApplicationQuit()
    {
        GameData.Save(new SaveData());
    }

    void Start()
    {
        instance = this;

        if (plantGrown)
        {
            grownPlants.Add(currentPlant);
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
        GVar.experience = data.experience;
        GVar.gardenSize = data.gardenSize;
        GVar.completedGoals = data.completedGoals;
        GVar.completedTasks = data.completedTasks;
        levelText.text = GVar.completedGoals.Count.ToString();
        grownPlants = PlantDataBase.instance.ConvertToData(data.plantData);
        moneyText.text = money.ToString();

        //Create layout
        flowerParent.GetComponent<HorizontalObjectLayout>().UpdateLayout(GVar.gardenSize);
        maxScrollDist = (flowerParent.GetComponent<HorizontalObjectLayout>().GetTotalWidth() / 2) - 4f;
        plots = flowerParent.GetComponentsInChildren<GardenPlot>();

        plots = plots.OrderBy(p => p.transform.position.x).ToArray();
        for (int i = 0; i < plots.Length; i++)
        {
            plots[i].index = i;
        }


        if (plantGrown)
        {
            Vector3 focusPosition = Camera.main.transform.position;
            focusPosition.x = plots[currentPlant.plotIndex].transform.position.x;
            focusPosition.x = Mathf.Abs(focusPosition.x) > maxScrollDist ? maxScrollDist * Mathf.Sign(focusPosition.x) : focusPosition.x;
            Camera.main.transform.position = focusPosition;
            waitTime = -2f;
            SelectObject(GetNearestObject(Camera.main.transform.position, null), Camera.main.transform.position);
        }
        else
        {
            SelectObject(GetNearestObject(Camera.main.transform.position, null), Camera.main.transform.position);
            Vector3 target = Camera.main.transform.position;
            target.x = selectedPlotArea.transform.position.x;
            Camera.main.transform.position = target;
        }
        plantGrown = false;



        foreach (PlantData p in grownPlants)
        {
            PlacePlant(p, p.plotIndex);
        }

        if (data.harvestedFlowers != null) GVar.harvestedFlowers = data.harvestedFlowers;
        GVar.collectiblesFound = data.collectiblesFound;

        if (GVar.playerSeedsIndex == null || GVar.playerSeedsIndex.Count < 2)
        {
            GVar.playerSeedsIndex = new List<int>();
            GVar.playerSeedsIndex.Add(3);
        }

        if (TaskManager.instance.GetCompletedTasks().Count > 0)
        {
            tasksCompleteIndicator.GetComponent<Animator>().enabled = true;
        }
    }

    void Update()
    {


        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, flowerLayer);
        if (Input.GetKeyUp(KeyCode.Mouse0) && !seedMenu.activeSelf && hit.collider && !holding) //Hit a plot -> Select
        {
            hit.transform.GetComponent<GardenPlot>().SelectPlot();
        }
        else if(Input.GetKeyDown(KeyCode.Mouse0) && !seedMenu.activeSelf) //Hit background -> Start scrolling
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, backLayer);
            if (hit.collider)
            {

                previousTouchPosition = Input.mousePosition;
                scrolling = true;
            }
        }
        if (Input.GetKey(KeyCode.Mouse0) && !seedMenu.activeSelf)
        {
            holdTime += Time.deltaTime;
            if (holdTime > 0.4f || Mathf.Abs(previousTouchPosition.x - Input.mousePosition.x) > 10)
            {
                holding = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !seedMenu.activeSelf && waitTime >= 0)
        {
            DragUI();
        }

        //TODO:
        //SHOULD DELETE THIS CODE, BUT FOR SOME REASON THAT BREAKS FLOWER HARVESTING!!??

        //Takes position between frames and translates picture in a shitty way
        if (scrolling)
        {
            //Have to change this at some point.. do I?
            scrollDirection = previousTouchPosition.x - Input.mousePosition.x;
            scrollDirection = Mathf.Abs(scrollDirection) > 200 ? 200 * Mathf.Sign(previousTouchPosition.x - Input.mousePosition.x) : scrollDirection;

            previousTouchPosition = Input.mousePosition;
        }

        //Move to direction and slow down over time
        scrollDirection = Camera.main.transform.position.x + Mathf.Sign(scrollDirection) * 0.2f > maxScrollDist ? 0 : scrollDirection;
        scrollDirection = Camera.main.transform.position.x + Mathf.Sign(scrollDirection) * 0.2f < -maxScrollDist ? 0 : scrollDirection;
        scrollDirection = Mathf.MoveTowards(scrollDirection, 0, Time.deltaTime * 100);

        //Make the sides of the screen bouncy
        if (Camera.main.transform.position.x < -maxScrollDist)
        {
            Vector3 targetpos = Camera.main.transform.position;
            targetpos.x = -maxScrollDist;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetpos, Time.deltaTime * 10);
        }
        else if (Camera.main.transform.position.x > maxScrollDist)
        {
            Vector3 targetpos = Camera.main.transform.position;
            targetpos.x = maxScrollDist;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetpos, Time.deltaTime * 10);
        }


        if (waitTime < 0)
        {
            waitTime += Time.deltaTime;
        }
        else
        {
            Vector3 target = Camera.main.transform.position;
            target.x = selectedPlotArea.transform.position.x;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target, Time.deltaTime * 4);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            scrolling = false;
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

        //Update money to make it fancier
        tempMoney = Mathf.Lerp(tempMoney, moneyVisual, Time.deltaTime * 7);
        moneyText.text = (Mathf.RoundToInt(tempMoney)).ToString();

        if (tutorialStillOn)
        {
            StartCoroutine(gardenTutorial());
        }

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


    //REMOVE THIS LATER (also change the harvestable to private later)
    int cheatcount = 0;
    public void Cheat()
    {
        cheatcount++;
        foreach (GardenPlot plot in plots)
        {
            plot.readyTime = System.DateTime.Now;
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

    public void OpenTaskScreen()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
        SceneManager.LoadScene("Tasks");
    }

    IEnumerator gardenTutorial()
    {
        yield return new WaitForSeconds(1);
        flowerPointer.SetActive(true);

        yield return new WaitForSeconds(2);
        flowerPointer.SetActive(false);

        yield return new WaitForSeconds(1);
        shopPointer.SetActive(true);

    }

}
