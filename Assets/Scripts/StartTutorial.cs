using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTutorial : MonoBehaviour
{
    public void StartTutorialCheck()
    {
        if (GVar.newSave)
        {
            Tutorial.tutorialOn = true;
        }
        if (!GVar.newSave)
        {
            Tutorial.tutorialOn = false;
        }
    }
}
