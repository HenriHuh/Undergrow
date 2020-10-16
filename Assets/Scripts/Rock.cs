using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rock : Interactable
{
    public GameObject wholeRock;
    int health = 2;
    private bool damaged = false;

    private void OnEnable()
    {
        wholeRock.SetActive(true);
        health = 2;
    }

    public override void Interact()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.hitRock);
        DrawTree.instance.DamageNearby();
        if (health > 0)
        {
            DrawTree.instance.ReloadCheckpoint(false);
        }
    }

    public override void Damage()
    {

        if (damaged) return;
        else StartCoroutine(WaitForFrameEnd());

        health--;
        if (health < 1)
        {
            EffectManager.instance.PlayParticle(EffectManager.instance.rockDestroy, transform.position);
            MapGen.instance.RemoveNode(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            wholeRock.SetActive(false);
        }
    }

    IEnumerator WaitForFrameEnd()
    {
        damaged = true;
        yield return new WaitForEndOfFrame();
        damaged = false;
        yield return null;
    }
}
