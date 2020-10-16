using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToUIPosition : MonoBehaviour
{
    public GameObject uiObject;
    void Update()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(uiObject.transform.position);
        vec.z = 0;
        transform.position = vec;
    }
}
