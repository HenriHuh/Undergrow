using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Tileset", order = 1,menuName ="ScriptableObjects/Tileset")]
public class DynamicTile : ScriptableObject
{
    public Sprite center;
    public Sprite single;
    public Sprite topLeft;
    public Sprite topRight;
    public Sprite bottomLeft;
    public Sprite bottomRight;
    public Sprite sideLeft;
    public Sprite sideRight;
    public Sprite sideTop;
    public Sprite sideBottom;
    public Sprite sideLeftRight;
    public Sprite sideTopBottom;
    public Sprite endLeft;
    public Sprite endRight;
    public Sprite endUp;
    public Sprite endDown;


}
