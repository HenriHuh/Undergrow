using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectible : Interactable
{
    [HideInInspector] public int id;

    private void OnEnable()
    {
        id = Random.Range(0, GVar.currentPlant.flowers.Count);
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GVar.currentPlant.flowers[id].icons[GVar.currentPlant.flowers[id].icons.Count - 1];
    }

    public override void Interact()
    {
        GVar.collectiblesFound++;
        SoundManager.instance.PlaySound(SoundManager.instance.collectSeed);
        if (GardenManager.currentPlant.flowerCounts[id] < GVar.currentPlant.flowers[id].icons.Count - 1)
        {
            GardenManager.currentPlant.flowerCounts[id]++;
        }
        //else
        //{
        //    GardenManager.money += GVar.currentPlant.flowers[id].value / 2;
        //    if (GVar.harvestedFlowers.ContainsKey(GVar.currentPlant.flowers[id].index))
        //    {
        //        GVar.harvestedFlowers[GVar.currentPlant.flowers[id].index]++;
        //    }
        //    else
        //    {
        //        GVar.harvestedFlowers.Add(GVar.currentPlant.flowers[id].index, 1);
        //    }
        //}
        GameManager.instance.CollectItem(id);
        EffectManager.instance.PlayParticle(EffectManager.instance.seedCollected, transform.position);
        gameObject.SetActive(false);
    }
}
