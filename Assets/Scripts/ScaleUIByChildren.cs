using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleUIByChildren : MonoBehaviour
{

    void Start()
    {
        UpdateLayout();
    }

    void Update()
    {
        UpdateLayout();
    }


    void UpdateLayout()
    {
        Vector2 size = transform.GetComponent<RectTransform>().sizeDelta;
        size.y = 0;
        foreach (Transform t in transform)
        {
            size.y += t.GetComponent<RectTransform>().sizeDelta.y + transform.GetComponent<VerticalLayoutGroup>().spacing;
        }

        transform.GetComponent<RectTransform>().sizeDelta = size;
    }
}
