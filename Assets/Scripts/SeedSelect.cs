using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Handles Seed selection UI thingie

public class SeedSelect : MonoBehaviour
{
    public GameObject seedButtonPrefab;
    public float padding;
    public Image silhouette;
    public GameObject layoutBackground;

    private GameObject selected;
    private Coroutine dragRoutine;

    bool scrollCooldown;

    //TODO: 
    //  1. Close window if clicked outside of it or smth

    void OnEnable()
    {
        padding = 0.35f * Screen.width;
        UpdateSeeds();
    }

    private void OnDisable()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DragUI(); //!!!!!!!!!!!!!

            /*PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == layoutBackground)
                    {
                        DragUI();
                    }
                }
            }*/
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
            SelectObject(GetNearestObject(selected.transform.position + Vector3.right * dir * 20, selected.transform));
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
        GameObject nearest = transform.GetChild(0).gameObject;
        float distance = Mathf.Infinity;
        foreach (Transform t in transform)
        {
            if(Vector3.Distance(t.position, origin) < distance && t != ignore)
            {
                nearest = t.gameObject;
                distance = Vector3.Distance(t.position, origin);
            }
        }
        return nearest;

    }

    void UpdateSeeds() //There should be 3 objects visible in the UI
    {
        int count = -2;

        GVar.playerSeedsIndex.OrderBy(i => PlantDataBase.instance.GetPlantByIndex(i).plantVariables.depth);
        //Loop through all seeds so the counts and crap work
        foreach (int i in GVar.playerSeedsIndex)
        {
            if (GVar.playerSeedsIndex.Count < 1) break;
            bool contains = false;

            foreach (Transform t in transform)
            {
                if (t.GetComponent<SeedUIContainer>().plantName == PlantDataBase.instance.GetName(i))
                {
                    t.GetComponent<SeedUIContainer>().count++;
                    t.GetComponentInChildren<Text>().text = (t.GetComponent<SeedUIContainer>().count + 1).ToString();
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                GameObject g = Instantiate(seedButtonPrefab, transform);
                g.GetComponent<SeedUIContainer>().targetPos = g.transform.position + ( padding) * count * Vector3.right;
                g.transform.position += (padding) * count * Vector3.right;
                g.GetComponent<SeedUIContainer>().plantName = PlantDataBase.instance.GetName(i);
                g.GetComponent<Button>().onClick.AddListener(() => SelectObject(g));
                g.GetComponentInChildren<Text>().text = "1";
                if (i == 3) g.GetComponentInChildren<Text>().text = ""; //No number for tutorial seed since it's always available
                g.GetComponent<Image>().sprite = PlantDataBase.instance.GetPlantByIndex(i).plantVariables.seedSprite;

                Color clr = Color.white;
                clr.a = 0.5f;
                g.GetComponentInChildren<Text>().color = clr;
                g.GetComponent<Image>().color = clr;
                count++;

            }
        }


        if (transform.childCount > 2 && transform.childCount < 5) //Must create dublicates to make the UI continuous
        {
            Button[] trans = transform.GetComponentsInChildren<Button>();
            foreach (Button t in trans)
            {
                GameObject g = Instantiate(t.gameObject, transform);
                g.GetComponent<SeedUIContainer>().targetPos = g.transform.position + (padding) * count * Vector3.right;
                g.transform.position = g.transform.position + (padding) * count * Vector3.right;
                g.GetComponent<Button>().onClick.AddListener(() => SelectObject(g));
                count++;
            }
        }
        else if (transform.childCount < 3) //Move objects towards center if there are less than 3 objects
        {
            Button[] trans = transform.GetComponentsInChildren<Button>();
            foreach (Button t in trans)
            {
                t.transform.position = t.transform.position + Vector3.right * (padding);
                t.GetComponent<SeedUIContainer>().targetPos = t.transform.position + Vector3.right * (padding);
            }
        }

        if (transform.childCount > 2)
        {
            selected = transform.GetChild(2).gameObject;
            selected.GetComponent<Image>().color = Color.white;

            //A lazy solution so the easiest plant would be listed first
            //TODO: Propably need to do some better solution here
            SelectObject(transform.GetChild(3).gameObject);
        }
        else if (transform.childCount > 0)
        {
            selected = transform.GetChild(0).gameObject;
            selected.GetComponent<Image>().color = Color.white;
        }

        silhouette.sprite = PlantDataBase.instance.GetPlantByName(selected.GetComponent<SeedUIContainer>().plantName).plantVariables.silhouette;

    }

    public void StartGame()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.startRoot);
        GardenManager.instance.SelectSeed(selected.GetComponent<SeedUIContainer>().plantName);
    }

    IEnumerator ScrollCooldown()
    {
        scrollCooldown = true;
        yield return new WaitForEndOfFrame();
        while (Vector3.Distance(selected.transform.position, selected.GetComponent<SeedUIContainer>().targetPos) > 8)
        {
            yield return null;
        }

        foreach (Transform t in transform)
        {
            t.transform.position = t.GetComponent<SeedUIContainer>().targetPos;
        }
        scrollCooldown = false;
        yield return null;
    }

    public void SelectObject(GameObject obj)
    {
        if (scrollCooldown) return;
        StartCoroutine(ScrollCooldown());

        if (obj == selected)
        {
            GardenManager.instance.SelectSeed(obj.GetComponent<SeedUIContainer>().plantName);
            return;
        }

        SoundManager.instance.PlaySound(SoundManager.instance.selectSeed);
        int dir = 0;
        if (obj.transform.position.x > transform.position.x)
        {
            dir = -1;
        }
        else
        {
            dir = 1;
        }

        foreach (Transform t in transform)
        {
            t.GetComponent<SeedUIContainer>().targetPos = t.position + Vector3.right * dir * (padding);
            Color clr = Color.white;
            clr.a = 0.5f;
            t.GetComponent<Image>().color = clr;
            t.GetComponentInChildren<Text>().color = clr;
        }

        selected = obj;
        selected.GetComponent<Image>().color = Color.white;
        selected.GetComponentInChildren<Text>().color = Color.white;


        if (transform.childCount > 2)
        {
            if (dir < 0)
            {
                //Set position
                transform.GetChild(0).position
                    = transform.GetChild(transform.childCount - 1).position;
                //Set target position
                transform.GetChild(0).GetComponent<SeedUIContainer>().targetPos
                    = transform.GetChild(transform.childCount - 1).position;

                transform.GetChild(0).SetAsLastSibling();
            }
            else
            {
                //Set position
                transform.GetChild(transform.childCount - 1).position
                    = transform.GetChild(0).position;
                //Set target position
                transform.GetChild(transform.childCount -1).GetComponent<SeedUIContainer>().targetPos
                    = transform.GetChild(0).position;

                transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
            }
        }

        silhouette.sprite = PlantDataBase.instance.GetPlantByName(selected.GetComponent<SeedUIContainer>().plantName).plantVariables.silhouette;
    }


}
