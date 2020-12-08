using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleChest : Interactable
{
    private void OnEnable()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ItemManager.instance.GetItemByType(Item.Type.Chest).icon;
    }

    public override void Interact()
    {
        GVar.playerItemsIndex.Add(ItemManager.instance.GetIndexByType(Item.Type.Chest));
        SoundManager.instance.PlaySound(SoundManager.instance.completeLevel);
        DrawTree.instance.CollectEffectPlay();
        EffectManager.instance.PlayParticle(EffectManager.instance.seedCollected, transform.position);
        GardenManager.chestFound = true;

        gameObject.SetActive(false);
    }
}
