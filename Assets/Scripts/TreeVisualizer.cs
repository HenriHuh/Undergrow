using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeVisualizer : MonoBehaviour
{
    //Assign in editor
    public Transform lineParent;
    [Range(0, 1)] public float angleFrequency;
    [Range(0, 1)] public float randomSpread;
    [Range(0, 1)] public float lineWidth;
    public Renderer backOverlay;
    public Light endLight;

    //Other
    LineRenderer[] lines;
    public static TreeVisualizer instance;
    int lineIndex = 1;
    LineRenderer currentLine;
    float nextAngle;
    Vector3 currentPosition;
    Vector3 randomPosition;
    [HideInInspector] public List<GameObject> flowers = new List<GameObject>();


    void Start()
    {
        lines = gameObject.GetComponentsInChildren<LineRenderer>();
        instance = this;
        currentLine = lines[1];
    }

    private void Update()
    {
        backOverlay.material.color = Color.Lerp(Color.clear, Color.black,-1.25f + (GVar.currentPlant.depth + currentPosition.y) / GVar.currentPlant.depth);

        endLight.range = (10 - (Mathf.Abs(GVar.currentPlant.depth) - Mathf.Abs(currentPosition.y))) / 8;
        if (Mathf.Abs(GVar.currentPlant.depth) - Mathf.Abs(currentPosition.y) < 0)
        {
            endLight.gameObject.SetActive(false);
        }
    }

    public void SetPosition(Vector3 target)
    {
        currentPosition = target;
        currentLine.SetPosition(currentLine.positionCount - 1, currentPosition);

    }

    void RandomizeNextPosition(Vector3 target)
    {
        randomPosition = Tools.CircularRandom(Vector3.zero, randomSpread);
    }

    public void SetNode(Vector3 target)
    {
        currentLine.positionCount += 1;
        currentLine.SetPosition(currentLine.positionCount - 1, currentPosition);
        RandomizeNextPosition(target);
    }


    public void SetNewLine(Vector3 position)
    {
        lineIndex = lineIndex + 1 >= lines.Length - 1 ? 0 : lineIndex + 1;
        currentLine = lines[lineIndex];
        currentLine.positionCount = 2;
        currentLine.SetPosition(0, position);
        currentLine.SetPosition(1, position);
    }

    public void SetBranch(Vector3 pos)
    {
        lineIndex = lineIndex + 1 >= lines.Length - 1 ? 0 : lineIndex + 1;
        lines[lineIndex].positionCount = 1;
        lines[lineIndex].SetPosition(0, pos);
        lines[lineIndex].startWidth = lineWidth * Random.Range(1.3f, 1.6f);
        StartCoroutine(GrowNewBranch(lines[lineIndex], 1, randomSpread));
    }

    IEnumerator GrowBranch(LineRenderer branch, float length, float spread)
    {
        float t = 0;
        Vector3 targetPos = branch.GetPosition(0) + Vector3.down * length + new Vector3(Random.Range(-length, length), Random.Range(-length / 2, length / 2));
        float sproutSpeed = Random.Range(0.8f, 1.0f) * DrawTree.instance.speedMultiplier * GVar.currentPlant.speed;
        int randCount = Random.Range(2, 6);

        for (int i = 0; i < randCount; i++)
        {

            branch.positionCount++;
            branch.SetPosition(branch.positionCount - 1, branch.GetPosition(branch.positionCount - 2));
            Vector3 nextPos = Vector3.MoveTowards(branch.GetPosition(0), targetPos, Vector3.Distance(branch.GetPosition(0), targetPos) * ((i + 1f) / randCount));
            nextPos = Tools.CircularRandom(nextPos, spread);
            while (t < 0.3f && Vector3.Distance(branch.GetPosition(branch.positionCount - 1), nextPos) > 0.03f)
            {
                branch.SetPosition(i + 1, Vector3.MoveTowards(branch.GetPosition(branch.positionCount - 1), nextPos, Time.deltaTime * sproutSpeed));
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;
            yield return null;
        }

        DrawTree.instance.AddCheckPoint(branch.GetPosition(branch.positionCount - 1));

        yield return null;
    }

    IEnumerator GrowNewBranch(LineRenderer branch, float length, float spread)
    {
        float t = 0;
        length *= 0.75f;
        float sproutSpeed = Random.Range(0.8f, 1.0f) * DrawTree.instance.speedMultiplier * GVar.currentPlant.speed;
        int randCount = Random.Range(2, 4);
        Vector3 prevPos = Vector3.zero;

        for (int i = 0; i < randCount; i++)
        {

            branch.positionCount++;
            branch.SetPosition(branch.positionCount - 1, branch.GetPosition(branch.positionCount - 2));

            //Vector3 dir = branch.GetPosition(branch.positionCount - 2) - branch.GetPosition(branch.positionCount - 1);
            Vector3 dir = prevPos != Vector3.zero ? branch.GetPosition(i) - prevPos : Vector3.down;
            prevPos = branch.GetPosition(i);
            Vector3 nextPos = branch.GetPosition(i) + dir * length + new Vector3(Random.Range(-length, length), Random.Range(-length / 2, length / 2));
            nextPos = Tools.AvoidanceRaycast(branch.GetPosition(i), nextPos, DrawTree.instance.interactableLayer);
            while (t < 0.3f && Vector3.Distance(branch.GetPosition(branch.positionCount - 1), nextPos) > 0.03f)
            {
                branch.SetPosition(i + 1, Vector3.MoveTowards(branch.GetPosition(branch.positionCount - 1), nextPos, Time.deltaTime * sproutSpeed));
                t += Time.deltaTime;
                yield return null;
            }
            RandomFlower(branch.GetPosition(branch.positionCount - 1), branch.GetPosition(branch.positionCount - 2), 3);
            t = 0;
            yield return null;
        }

        DrawTree.instance.AddCheckPoint(branch.GetPosition(branch.positionCount - 1));

        yield return null;
    }

    IEnumerator FinishBranch(LineRenderer branch, float length, float spread)
    {
        float t = 0;
        length *= 0.75f;
        float sproutSpeed = Random.Range(0.8f, 1.0f) * DrawTree.instance.speedMultiplier * GVar.currentPlant.speed;
        int randCount = Random.Range(6, 8);
        Vector3 prevPos = Vector3.zero;

        for (int i = 0; i < randCount; i++)
        {

            branch.positionCount++;
            branch.SetPosition(branch.positionCount - 1, branch.GetPosition(branch.positionCount - 2));

            //Vector3 dir = branch.GetPosition(branch.positionCount - 2) - branch.GetPosition(branch.positionCount - 1);
            Vector3 dir = prevPos != Vector3.zero ? branch.GetPosition(i) - prevPos : Vector3.down;
            prevPos = branch.GetPosition(i);
            Vector3 nextPos = branch.GetPosition(i) + dir * length + new Vector3(Random.Range(-length, length), Random.Range(-length / 2, length / 2));
            nextPos = Tools.AvoidanceRaycast(branch.GetPosition(i), nextPos, DrawTree.instance.interactableLayer);
            while (t < 0.3f && Vector3.Distance(branch.GetPosition(branch.positionCount - 1), nextPos) > 0.03f)
            {
                branch.SetPosition(i + 1, Vector3.MoveTowards(branch.GetPosition(branch.positionCount - 1), nextPos, Time.deltaTime * sproutSpeed));
                t += Time.deltaTime;
                yield return null;
            }
            RandomFlower(branch.GetPosition(branch.positionCount - 1), branch.GetPosition(branch.positionCount - 2), 3);
            t = 0;
            yield return null;
        }

        yield return null;
    }

    public void AddFlower(GameObject flower) //Add new flower to the list of objects that can be instantiated
    {
        if (flowers.Find(f => f.name == flower.name)) return;

        flowers.Add(flower);

    }

    public void FinishBranch() //Finish root when game is won
    {
        LineRenderer line = Instantiate(lines[0]);
        line.positionCount = 1;
        line.SetPosition(0, currentPosition);
        line.endWidth /= 2;
        StartCoroutine(FinishBranch(line, 2.5f, 0.35f));
    }


    public void RandomFlower(Vector3 start, Vector3 end, int change)
    {
        for (int i = 0; i < flowers.Count; i++)
        {
            if (Random.Range(-change, 10 - GameManager.instance.collectedFlowers[i]) <= 0) //this is not right but whatever
            {
                Vector3 position = Vector3.MoveTowards(start, end, Random.Range(0, Vector3.Distance(start, end)));

                GameObject flower = Instantiate(
                    flowers[i], //Choose random flower 
                    position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0), //Random Position
                    new Quaternion(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0, 0)); //Random Rotation
                TemporaryObjects.instance.AddItem(flower.gameObject);
                break;
            }
        }
    }
}
