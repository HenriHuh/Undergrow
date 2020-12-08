using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//General tools for math and shit
//Add static methods that you use regularly here

public static class Tools
{

    /// <summary>
    /// Get a random point in a circular area
    /// </summary>
    public static Vector3 CircularRandom(Vector3 origin, float radius)
    {
        Vector3 vec = new Vector3();
        float r = radius * Mathf.Sqrt(Random.Range(0f, 1f));
        float t = Random.Range(0f, 1f) * 2f * Mathf.PI;
        vec.x = r * Mathf.Cos(t);
        vec.y = r * Mathf.Sin(t);
        return origin + vec;
    }

    /// <summary>
    /// Get a random point in a circular area rounded to integer
    /// </summary>
    public static Vector3 CircularRandomRounded(Vector3 origin, float radius)
    {
        Vector3 vec = new Vector3();
        float r = radius * Mathf.Sqrt(Random.Range(0f, 1f));
        float t = Random.Range(0f, 1f) * 2f * Mathf.PI;
        vec.x = (int)(r * Mathf.Cos(t));
        vec.y = (int)(r * Mathf.Sin(t));
        return origin + vec;
    }

    /// <summary>
    /// Get a random point in a square area
    /// </summary>
    public static Vector3 SquareRandom(Vector3 origin, float spread)
    {
        return origin + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
    }

    /// <summary>
    /// Get a random point in a square area rounded to integer
    /// </summary>
    public static Vector3 SquareRandomRounded(Vector3 origin, float spread)
    {
        return origin + new Vector3((int)Random.Range(-spread, spread), (int)Random.Range(-spread, spread), 0);
    }

    /// <summary>
    /// Random distributed downwards
    /// </summary>
    public static int ExponentialRandom(int a, int b)
    {

        //killlmeeeeeeeeeee
        return Mathf.FloorToInt(Mathf.Pow(Random.Range(0f, 1f), 2) * (b - a) + a);
    }

    /// <summary>
    /// Move the last item in a list to be the first
    /// </summary>
    public static List<GameObject> LastToFirst(List<GameObject> list)
    {
        GameObject temp = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        list.Insert(0, temp);
        return list;
    }

    /// <summary>
    /// Move the first item in a list to be the last
    /// </summary>
    public static List<GameObject> FirstToLast(List<GameObject> list)
    {
        GameObject temp = list[0];
        list.RemoveAt(0);
        list.Add(temp);
        return list;
    }

    /// <summary>
    /// Formats datetime to MMDDHHMMSS
    /// </summary>
    public static int TimeToInt(System.DateTime time)
    {
        List<string> values = new List<string>();
        values.Add(time.Month.ToString());
        values.Add(time.Day.ToString());
        values.Add(time.Hour.ToString());
        values.Add(time.Minute.ToString());
        values.Add(time.Second.ToString());

        for (int i = 0; i < values.Count; i++)
        {
            if (values[i].Length < 2)
            {
                values[i] = values[i].Insert(0, "0");
            }
        }
        string finalString = "";
        foreach (string t in values)
        {
            finalString += t;
        }
        return int.Parse(finalString);
    }

    /// <summary>
    /// Get datetime from int format MMDDHHMMSS
    /// </summary>
    public static System.DateTime IntToTime(int time)
    {
        List<string> values = new List<string>();
        string timeString = time.ToString();

        if (timeString.Length < 10) return new System.DateTime();

        for (int i = 0; i < 5; i++)
        {
            values.Add(timeString.Substring(i * 2, 2));
        }
        return new System.DateTime(System.DateTime.Now.Year, int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]));
    }


    /// <summary>
    /// Turn vector away from obstacles
    /// </summary>
    public static Vector3 AvoidanceRaycast(Vector3 start, Vector3 end, LayerMask layer)
    {
        List<Vector3> vectors =  new List<Vector3>();
        List<float> distances = new List<float>();
        Vector3 dir = end - start;

        RaycastHit hit;
        if (!Physics.Linecast(start, end, out hit, layer)) return end;
        else
        {
            vectors.Add(end);
            distances.Add(Vector3.Distance(start, hit.point));
        }

        Vector3 sideVec = end + new Vector3(dir.y, -dir.x) * 0.5f;
        if (!Physics.Linecast(start, sideVec, out hit, layer)) return sideVec;
        else
        {
            vectors.Add(sideVec);
            distances.Add(Vector3.Distance(start, hit.point));
        }

        sideVec = end + new Vector3(-dir.y, dir.x) * 0.5f;
        if (!Physics.Linecast(start, sideVec, out hit, layer)) return sideVec;
        else
        {
            vectors.Add(sideVec);
            distances.Add(Vector3.Distance(start, hit.point));
        }
        vectors.OrderBy(v => Vector3.Distance(start, v));
        Debug.DrawLine(start, vectors[0], Color.green, 5);
        Debug.DrawLine(start, vectors[2], Color.red, 5);
        return vectors[0];
    }
}
