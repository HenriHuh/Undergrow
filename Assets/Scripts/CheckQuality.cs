using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckQuality : MonoBehaviour
{
    public GVar.Quality quality;
    public bool disableObject = true;
    public UnityEvent toInvoke;

    void Start()
    {
        if ((int)GVar.quality >= (int)quality)
        {
            if(disableObject) gameObject.SetActive(false);
            toInvoke.Invoke();
        }
    }
}
