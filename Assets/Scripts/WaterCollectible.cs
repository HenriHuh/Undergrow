using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollectible : MonoBehaviour
{
    private void OnEnable()
    {
        DrawTree.instance.waterDrops.Add(transform);
    }
    private void OnDisable()
    {
        DrawTree.instance.waterDrops.Remove(transform);
    }
}
