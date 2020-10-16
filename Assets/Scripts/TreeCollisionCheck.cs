using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

//Static class for checking tree collisions using raycast
//Branch positions/vertices are stored in a Vector3 list

public class TreeCollisionCheck : MonoBehaviour
{

    static int maxBranchCount = 4;
    static int maxVertexCount = 8;
    public static List<Branch> branches =  new List<Branch>();
    static Branch currentBranch;
    

    public static void AddBranch(Vector3 position)
    {
        if (branches.Count >= maxBranchCount)
        {
            branches.RemoveAt(0);
        }

        branches.Add(new Branch());
        currentBranch = branches[branches.Count - 1];
        currentBranch.positions.Add(position);
    }

    public static void AddPosition(Vector3 position)
    {
        if (currentBranch.positions.Count > maxVertexCount)
        {
            currentBranch.positions.RemoveAt(0);
        }
        currentBranch.positions.Add(position);
    }

    public static bool CheckCurrentCollision(LayerMask layer)
    {
        if (currentBranch.CheckCollision(layer)) return true;
        return false;
    }

    public static bool CheckCurrentCollision(LayerMask layer, GameObject obj)
    {
        if (currentBranch.CheckCollision(layer, obj)) return true;
        return false;
    }

    public static bool CheckCollision(LayerMask layer)
    {
        foreach (Branch b in branches)
        {
            if (b.CheckCollision(layer)) return true;
        }
        return false;
    }

    public static bool CheckCollision(LayerMask layer, GameObject obj)
    {
        foreach (Branch b in branches)
        {
            if (b.CheckCollision(layer, obj)) return true;
        }
        return false;
    }

}

public class Branch
{
    public List<Vector3> positions = new List<Vector3>();

    public bool CheckCollision(LayerMask layer)
    {
        for (int i = 0; i < positions.Count - 2; i++)
        {
            if (Physics.Linecast(positions[i], positions[i+1], layer)) return true;
        }
        return false;
    }
    public bool CheckCollision(LayerMask layer, GameObject obj)
    {
        RaycastHit hit;
        for (int i = 0; i < positions.Count - 2; i++)
        {
            if (Physics.Linecast(positions[i], positions[i + 1], out hit, layer) 
                && hit.transform.gameObject == obj) return true;
        }
        return false;
    }
}
