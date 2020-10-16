using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnim : MonoBehaviour
{
    //Growing animation for the flowers


    float t;

    void Start()
    {
        t = Random.Range(0.40f, 0.50f);
    }

    void Update()
    {
        transform.localScale += transform.localScale * 10f * Time.deltaTime * t;
        t -= Time.deltaTime;

        if (t < 0f)
        {
            enabled = false;
        }
    }
}
