using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedUIContainer : MonoBehaviour
{

    [HideInInspector] public string plantName;
    [HideInInspector] public int count;
    [HideInInspector] public Vector3 targetPos;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10);
    }
}
