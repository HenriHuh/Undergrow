using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//1. Create pool of MapNode according to map nodes
//2. Instantiate nodes(objects) and move them to nodePool
//3. Randomize objects
//  3.1 Get area: Check area around current
//  3.2 If given position is around previous area -> don't instantiate
//  3.3 Randomize and create objects
//      3.3.1 Check if pool contains inactive object of type
//      3.3.2 Instantiate new object if not
//  3.4 Create cluster if cluster size is > 1
//  3.5 Check if created nodes type have cluster size > 1
//      3.5.2 If not -> Go to 4
//      3.5.1 If yes -> Add to list of nodes to tile
//          (Nodes in list are tiled every frame)
//          (Check for adjacent nodes and change sprite accordinly)
//4. Wait until certain distance is reached
//5. Back to 3

public class MapGen : MonoBehaviour
{
    //Assign in editor
    [Tooltip("Increase the amount of objects in the map.")]
    public int iterations;
    [Tooltip("Update when distance from last updatepoint exceeds this value.")]
    public float distanceToUpdate;
    [Tooltip("Radius to generate objects in.")]
    public float genRadius;
    [Tooltip("List of objects to create.")]
    public Plant currentPlant;


    //Static variables
    public static MapGen instance;
    public static List<MapNode> nodes = new List<MapNode>();
    public static List<MapNode> nodePool = new List<MapNode>();
    public static Vector3[] sides = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };


    //Private
    Vector3 previousUpdatePoint;
    List<MapNode> nodesToTile = new List<MapNode>();
    Coroutine tilingRoutine;

    bool started;

    //TODO: 
    //  1. Start generating earlier
    //  2. FIX OVERLAPPING SHIT

    //NOTE: List.Find() is super heavy to run so maybe change that later?


    void Awake()
    {
        previousUpdatePoint = Vector3.up * distanceToUpdate * 1.75f;
        instance = this;

        if (GVar.currentPlant == null) //For testing the tree scene
        {
            GVar.currentPlant = currentPlant.plantVariables;
            GardenManager.currentPlant = PlantDataBase.GetName(currentPlant.plantVariables);
        }


        nodes.Clear();
        nodePool.Clear();
        Invoke("LateStart", 0.25f);
    }

    void LateStart()
    {
        started = true;
    }

    void Update()
    {
        if (Vector3.Distance(previousUpdatePoint, DrawTree.instance.GetPosition()) > distanceToUpdate && started)
        {
            GenerateArea(DrawTree.instance.GetPosition());
        }

    }

    void GenerateArea(Vector3 point)
    {
        CleanMap(point);

        point.x = (int)point.x;
        point.y = (int)point.y;


        for (int i = 0; i < iterations; i++)
        {
            foreach (MapObject mapObj in GVar.currentPlant.mapObjects)
            {
                if (DrawTree.instance.GetPosition().y < mapObj.depth && Random.Range(0f,1f) < mapObj.rarity)
                {
                    Vector3 objPosition = Tools.CircularRandomRounded(point, genRadius);
                    if (Vector3.Distance(previousUpdatePoint, objPosition) > genRadius && nodes.Find(n => Equals(n.point, objPosition)) == null)
                    {
                        MapNode node = GetItemFromPool(mapObj);
                        node.gameObj.SetActive(true);
                        node.gameObj.transform.position = objPosition;
                        node.point = objPosition;
                        nodes.Add(node);

                        if (mapObj.maxClusterSize > 1)
                        {
                            nodesToTile.Add(node);
                            MakeCluster(node.point, mapObj.maxClusterSize, mapObj);
                        }
                    }
                }
            }
        }

        foreach (MapNode n in nodes)
        {
            if (Vector3.Distance(previousUpdatePoint, n.point) > genRadius * 0.75f && !nodesToTile.Contains(n))
            {
                nodesToTile.Add(n);
            }
        }

        if (tilingRoutine != null)
        {
            StopCoroutine(tilingRoutine);
        }
        tilingRoutine = StartCoroutine(CycleThroughTilesets());


        previousUpdatePoint = point;
    }

    public void MakeTile(GameObject obj)
    {
        MakeTile(nodes.Find(n => Equals(n.gameObj, obj)));
    }

    void MakeTile(MapNode node) //For single tiles
    {
        //sides = UP, DOWN, LEFT, RIGHT
        //Using bools instead of vectors for light weight
        //It's weird, I know
        MapObject mapObj = GVar.currentPlant.mapObjects.Find(m => m.type == node.type);

        List<bool> availables = new List<bool>();
        foreach (Vector3 side in sides)
        {
            if (nodes.Find(n => Equals(n.point, node.point + side) && Equals(n.type, node.type)) == null)
            {
                availables.Add(false);
            }
            else
            {
                availables.Add(true);
            }
        }

        List <Sprite> spriteToUse = new List<Sprite>();
        for (int i = 0; i < mapObj.tiles.Length; i++)
        {
            if (CompareBoolList(availables, new List<bool>() { true, true, true, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].center);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, true, true, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].sideRight);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, true, false, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].sideLeft);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, true, false, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].sideLeftRight);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, false, true, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].sideBottom);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, false, true, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].bottomRight);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, false, false, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].bottomLeft);
            }
            else if (CompareBoolList(availables, new List<bool>() { true, false, false, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].endDown);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, true, true, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].sideTop);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, true, true, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].topRight);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, true, false, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].topLeft);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, true, false, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].endUp);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, false, true, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].sideTopBottom);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, false, true, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].endRight);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, false, false, true }))
            {
                spriteToUse.Add(mapObj.tiles[i].endLeft);
            }
            else if (CompareBoolList(availables, new List<bool>() { false, false, false, false }))
            {
                spriteToUse.Add(mapObj.tiles[i].single);
            }
        }

        //Set sprite
        for (int i = 0; i < spriteToUse.Count; i++)
        {
            node.gameObj.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = spriteToUse[i];
        }

    }

    IEnumerator CycleThroughTilesets()
    {
        yield return new WaitForEndOfFrame();
        while (nodesToTile.Count > 0)
        {
            MakeTile(nodesToTile[0]);
            nodesToTile.RemoveAt(0);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    bool CompareBoolList(List<bool> a, List<bool> b)
    {
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveNode(GameObject obj)
    {
        MapNode node = nodes.Find(n => Equals(n.gameObj, obj));
        if (node == null) return;

        node.gameObj.SetActive(false);
        nodePool.Add(node);
        nodes.Remove(node);
    }

    void CleanMap(Vector3 point)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(point, nodes[i].gameObj.transform.position) > genRadius)
            {
                nodes[i].gameObj.SetActive(false);
                nodePool.Add(nodes[i]);
                nodes.RemoveAt(i);

            }
        }
    }

    /// <summary>
    /// Create a cluster around given point
    /// </summary>
    void MakeCluster(Vector3 origin, int maxSize, MapObject mapObj)
    {
        

        Vector3 currentPoint = origin;
        List<Vector3> positions = new List<Vector3>();
        while (positions.Count < maxSize)
        {
            if (Random.Range(0, maxSize - positions.Count) > 0)
            {
                List<Vector3> availableSides = new List<Vector3>();
                foreach (Vector3 s in sides)
                {
                    if (nodes.Find(n => n.point == origin + s) == null)
                    {
                        availableSides.Add(s);
                    }
                }
                if (availableSides.Count > 0)
                {
                    Vector3 p = currentPoint + availableSides[Random.Range(0, availableSides.Count - 1)];
                    MapNode node = GetItemFromPool(mapObj);
                    node.gameObj.SetActive(true);
                    node.gameObj.transform.position = p;
                    node.point = p;
                    nodes.Add(node);
                    positions.Add(p);
                    nodesToTile.Add(node);
                }
                else
                {
                    positions.Remove(currentPoint);
                }
            }
            else
            {
                break;
            }

            if (positions.Count == 0) break;
            currentPoint = positions[Tools.ExponentialRandom(0, positions.Count)];
        }

    }

    MapNode GetItemFromPool(MapObject mapObj)
    {
        MapNode node = nodePool.Find(n => n.type == mapObj.type);
        if (node == null)
        {
            node = new MapNode(mapObj.type, Instantiate(mapObj.gameObj));
        }
        nodePool.Remove(node);
        return node;
    }

}

[System.Serializable]
public class MapObject
{
    public MapNode.Type type;
    public GameObject gameObj;
    [Range(0.00f, 1.00f)] public float rarity;
    public int maxClusterSize;
    [Tooltip("Assign tiles here if objects are in clusters.")] public DynamicTile[] tiles;
    [Tooltip("Depth where objects start spawning. (e.g. -100)")] public float depth;
}


[System.Serializable]
public class MapNode
{
    public enum Type
    {
        rock,
        water,
        lava,
        seed,
        mole
    }

    public Vector3 point;
    public Type type;
    public GameObject gameObj;

    public MapNode(Type _type, GameObject _obj)
    {
        type = _type;
        gameObj = _obj;
        gameObj.SetActive(false);
    }
}
