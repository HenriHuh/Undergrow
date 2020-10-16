using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script for creating clusters of the gameobject

public class CreateCluster : MonoBehaviour
{
    public float waitTime;
    public GameObject objectPrefab;
    static Vector3[] vertical = {Vector3.up, Vector3.down};
    static Vector3[] horizontal = { Vector3.right, Vector3.left };

    void Start()
    {
        if (GameManager.gameTime < waitTime)
        {
            gameObject.SetActive(false);
            return;
        }

        foreach (Vector3 verPos in vertical)
        {
            if (Random.Range(0, 2) == 1)
            {
                Instantiate(objectPrefab, transform).transform.position = transform.position + verPos;
                foreach (Vector3 horPos in horizontal)
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        Instantiate(objectPrefab, transform).transform.position = transform.position + verPos + horPos;
                    }
                }
            }
        }
        foreach (Vector3 horPos in horizontal)
        {
            if (Random.Range(0, 2) == 1)
            {
                Instantiate(objectPrefab, transform).transform.position = transform.position + horPos;
            }
        }
    }
}
