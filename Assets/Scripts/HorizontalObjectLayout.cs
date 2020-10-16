using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HorizontalObjectLayout : MonoBehaviour
{
    public GameObject leftSide, rightSide;
    public GameObject middlePrefab;
    public int size = 1;
    [Range(0f,2f)] public float offset = 1;

    public void UpdateLayout(int _size)
    {
        size = _size;
        leftSide.transform.position = transform.position + Vector3.left * middlePrefab.GetComponent<Collider>().bounds.size.x * offset * (size + 1) / 2;
        for (int i = 0; i < size; i++)
        {
            GameObject g = Instantiate(middlePrefab, leftSide.transform.position + (1 + i) * Vector3.right * middlePrefab.GetComponent<Collider>().bounds.size.x * offset, quaternion.identity);
            g.transform.parent = transform;
        }
        rightSide.transform.position = transform.position + Vector3.right * middlePrefab.GetComponent<Collider>().bounds.size.x * offset * (size + 1) / 2;
    }

    public float GetTotalWidth()
    {
        float width = leftSide.GetComponent<Collider>().bounds.size.x;
        width += rightSide.GetComponent<Collider>().bounds.size.x;
        width += size * middlePrefab.GetComponent<Collider>().bounds.size.x;
        return width;
    }
}
