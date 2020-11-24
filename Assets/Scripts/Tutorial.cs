using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    //public Button tutorialButton;
    public static int launchCount;
    public GameObject handPointer1;
    public GameObject handPointer2;
    public GameObject handPointer3;
    public GameObject darkMode;
    public GameObject gardenLeft;
    public GameObject gardenRight;
    public GameObject gardenMiddle;
    public GameObject inventory;

    public bool activateGrowPointer;
    public static bool tutorialOn;
    bool firstPointer;

    Vector3 growButton;


    // Update is called once per frame
    void Update()
    {   if (tutorialOn)
        {
            if (firstPointer == false)
            {
                StartCoroutine(planting());
            }
            if (inventory.gameObject.activeSelf == true && !activateGrowPointer)
            {
                //Debug.Log("coroutine choose seed started");
                StartCoroutine(chooseSeed());
            }
            if (activateGrowPointer)
            {
                StartCoroutine(growSeed());
            }
        }
        
        
    }

    public void StartTutorialButton()
    {
        StartCoroutine(planting());
        tutorialOn = true;
    }
        
    IEnumerator planting()
    {
        yield return new WaitForSeconds(2);
        for (int i=0; i < gardenLeft.transform.childCount; i++)
        {
            var child = gardenLeft.transform.GetChild(i).gameObject;
            if (child != null)
            {
                child.SetActive(false);
            }
        }
        for (int i= 0; i < gardenRight.transform.childCount; i++)
        {
            var child = gardenRight.transform.GetChild(i).gameObject;
            if (child != null)
            {
                child.SetActive(false);
            }
        }
        for (int i = 0; i < gardenMiddle.transform.childCount; i++)
        {
            var child = gardenMiddle.transform.GetChild(i).gameObject;
            if (child != null)
            {
                child.SetActive(false);
            }
        }
        gardenMiddle.transform.GetChild(5).gameObject.SetActive(true);

        //probably play animation
        handPointer1.SetActive(true);
        darkMode.SetActive(true);
        firstPointer = true;
    }

    IEnumerator chooseSeed()
    {
        //firstPointer = false;
        handPointer1.SetActive(false);
        yield return new WaitForSeconds(1);
        
        //probably play animation
        handPointer2.SetActive(true);
        activateGrowPointer = true;
    }

    IEnumerator growSeed()
    {
        activateGrowPointer = true;
        StopCoroutine(chooseSeed());
        yield return new WaitForSeconds(2);
        handPointer2.SetActive(false);
        handPointer3.SetActive(true);
    }

}
