using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Interactable
{
    //Poison tiles

    public override void Interact()
    {
        DrawTree.instance.WaterDrain();
    }
}
