using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTutorial : MonoBehaviour
{
    public GameObject taskExplanationPanel;
    public GameObject levelExplanationPanel;
    public GameObject taskPointer;
    public GameObject levelPointer;

    bool taskPointerActive = false;
    bool levelPointerActive = false;
    

    // Update is called once per frame
    void Update()
    {
        if (GardenManager.tutorialPart2StillOn)
        {
            if (taskPointerActive == false)
            {
                StartCoroutine(taskExplanation());
            }
            if (taskPointerActive)
            {
                StartCoroutine(levelExplanation());
            }
            if (levelPointerActive)
            {
                StartCoroutine(closeExplanations());
            }
        }
        
    }
    
    IEnumerator taskExplanation()
    {
        yield return new WaitForSeconds(1);
        taskExplanationPanel.SetActive(true);
        taskPointer.SetActive(true);
        taskPointerActive = true;
    }

    IEnumerator levelExplanation()
    {
        yield return new WaitForSeconds(2);
        taskExplanationPanel.SetActive(false);
        taskPointer.SetActive(false);

        levelExplanationPanel.SetActive(true);
        levelPointer.SetActive(true);
        levelPointerActive = true;
    }

    IEnumerator closeExplanations()
    {
        yield return new WaitForSeconds(2);
        levelExplanationPanel.SetActive(false);
        levelPointer.SetActive(false);
        GardenManager.tutorialPart2StillOn = false;
    }
}

