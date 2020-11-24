using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can be used for prefabs and other objects to call soundmanager effects and stuff
public class SoundTrigger : MonoBehaviour
{
    public void PlayClick()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
    }

    public void PlayPop()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.uiPop);
    }
}
