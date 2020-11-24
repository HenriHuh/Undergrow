using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is used for collectable flowers
public class Collectible : Interactable
{
    [HideInInspector] public int id;

    private void OnEnable()
    {
        if (GVar.collectableFlowerIndex.Count > 0)
        {
            id = GVar.collectableFlowerIndex[Random.Range(0, GVar.collectableFlowerIndex.Count)];
        }
        else
        {
            id = Random.Range(0, GVar.currentPlant.flowers.Count);
        }
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GVar.currentPlant.flowers[id].icons[GVar.currentPlant.flowers[id].icons.Count - 1];
    }

    public override void Interact()
    {
        GVar.collectiblesFound++;
        SoundManager.instance.PlaySound(SoundManager.instance.collectSeed);
        if (GardenManager.currentPlant.flowerCounts[id] < GVar.currentPlant.flowers[id].icons.Count - 1)
        {
            GardenManager.currentPlant.flowerCounts[id]++;
            if (GardenManager.currentPlant.flowerCounts[id] >= GVar.currentPlant.flowers[id].icons.Count - 1)
            {
                GVar.collectableFlowerIndex.Remove(id);
            }
        }
        GameManager.instance.CollectItem(id);
        DrawTree.instance.CollectEffectPlay();
        EffectManager.instance.PlayParticle(EffectManager.instance.seedCollected, transform.position);
        gameObject.SetActive(false);
    }
}
