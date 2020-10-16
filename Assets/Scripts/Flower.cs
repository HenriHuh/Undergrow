using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flower", order = 1, menuName = "ScriptableObjects/Flower")]
public class Flower : ScriptableObject
{
    public string flowerName;
    public int value;
    public GameObject flower;
    public Material material;
    public List<Sprite> icons;
    public int index;
}