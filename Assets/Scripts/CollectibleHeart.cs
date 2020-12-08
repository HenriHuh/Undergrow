using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleHeart : Interactable
{
    public override void Interact()
    {
        DrawTree.instance.AddCPPoint();
        EffectManager.instance.PlayParticle(EffectManager.instance.seedCollected, transform.position);
        SoundManager.instance.PlaySound(SoundManager.instance.completeLevel);
        gameObject.SetActive(false);
    }
}
