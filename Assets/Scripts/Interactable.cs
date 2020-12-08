using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(CheckCollision());
    }

    //This is propably unnecessary
    IEnumerator CheckCollision()
    {
        yield return new WaitForEndOfFrame();
        if (Physics.CheckSphere(transform.position, 0.2f))
        {
            gameObject.SetActive(false);
        }
        yield return null;
    }

    public virtual void Interact()
    {

    }
    public virtual void Damage()
    {

    }
}
