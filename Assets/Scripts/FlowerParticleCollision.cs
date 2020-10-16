using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerParticleCollision : MonoBehaviour
{

    private void OnParticleCollision(GameObject other)
    {
        GardenManager.instance.PopInventory();
    }
}
