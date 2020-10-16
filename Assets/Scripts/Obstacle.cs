using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Interactable
{


    public override void Interact()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.hitPoison);
        DrawTree.instance.ReloadCheckpoint(false);
    }
}
