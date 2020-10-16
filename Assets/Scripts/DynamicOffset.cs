using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicOffset : MonoBehaviour
{
    Renderer mat;

    void Start()
    {
        mat = gameObject.GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        mat.material.SetTextureOffset("_MainTex", pos - transform.position);
    }
}