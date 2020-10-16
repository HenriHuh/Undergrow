using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawTree : MonoBehaviour
{
    //Assign in editor
    public LayerMask backLayer, interactableLayer, branchLayer;
    public Transform cameraFollow;
    public Text cpLeftText;
    public Text cpCountText;
    public Image waterFill;
    public Image depthFill;
    public Image cpReloadFill;
    public ParticleSystem diggingParticle;
    public static bool altControls = true;
    public Text debugText;
    [Range(0, 1)] public float distanceBetweenNodes;
    public GameObject branchHead;
    public TrailRenderer headTrailEffect;
    public float cpCooldown;
    public GameObject joystickVisual;
    public GameObject joystickPointer;
    public GameObject pointer;
    public GameObject waterPanel;
    public GameObject progressPanel;
    public GameObject obstaclesPanel;
    public GameObject collectablesPanel;

    //Other things
    Vector3 currentPosition;
    Vector3 previousPosition;

    float cpReloadTimer = Mathf.Infinity;
    float water = 80;
    float maxWater = 100;
    int branchRate = 9;
    int nextBranch = 0;
    Vector3 direction = Vector3.down;
    Vector3 randomPos = Vector3.zero;
    float maxDistance = 1f;
    [HideInInspector] public float speedMultiplier = 1f;
    Color headTrailColor;
    Vector3 joystickOrigin;

    List<Vector3> checkPoints = new List<Vector3>();
    int cpointsleft = 3;
    Coroutine hideTextRoutine;
    TrailRenderer tempTrail;
    bool invulnerable;

    public bool pointerShown;

    public static DrawTree instance;

    //TODO:
    //  1. Flower instantiation visual things
    //  2. Showing collected flowers at the end of the level

    void Start()
    {
        instance = this;
        UpdateUI();
        TreeCollisionCheck.AddBranch(Vector3.zero);

        diggingParticle.Play();
        headTrailColor = headTrailEffect.startColor;
        SoundManager.instance.PlayMusic(SoundManager.instance.musicTree);
    }

    void Update()
    {
        //Debug.Log(progressPanel.activeSelf);
        if (GameManager.instance.gameEnded)
        {
            diggingParticle.Stop();
            joystickVisual.SetActive(false);
            joystickPointer.SetActive(false);
            return;
        }

        if (Tutorial.tutorialOn)
        {
            if (pointerShown == false)
            {
                StartCoroutine(TutorialRoot());
            }
            if (pointerShown)
            {
                pointer.SetActive(false);
            }
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            joystickOrigin = Input.mousePosition;
            if(joystickOrigin.x + Screen.width/4 > Screen.width)
            {
                joystickOrigin.x = Screen.width - Screen.width / 4;
            }
            else if (joystickOrigin.x - Screen.width / 4 < 0)
            {
                joystickOrigin.x = Screen.width / 4;
            }
            if (joystickOrigin.y + Screen.height / 8 > Screen.height)
            {
                joystickOrigin.y = Screen.height - Screen.height / 8;
            }
            else if (joystickOrigin.y - Screen.height / 8 < 0)
            {
                joystickOrigin.y = Screen.height / 8;
            }

            joystickVisual.transform.position = joystickOrigin;
            joystickVisual.SetActive(true);
            joystickPointer.SetActive(true);

            if (direction == Vector3.down)
            {
                SwapDirection();
            }
            else
            {
                //StartCoroutine(WaitForDoubleTap());
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            joystickVisual.SetActive(false);
            joystickPointer.SetActive(false);
        }

        if (water > 0)
        {

            Vector3 target = currentPosition + -(joystickOrigin - Input.mousePosition).normalized;
            joystickPointer.transform.position = Vector3.MoveTowards(joystickVisual.transform.position, Input.mousePosition, Screen.width/8);
            if (target.y > currentPosition.y - 0.3f)
            {
                target.y = currentPosition.y - 0.3f;
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //target.y += 1f;
            }

            //Move the line position towards the target position
            if (altControls)
            {
                if (!Input.GetKey(KeyCode.Mouse0))
                {
                    target = currentPosition + Vector3.down * 1 + randomPos;
                }
                target =
                    Vector3.MoveTowards(currentPosition + Vector3.down, target,
                    Vector3.Distance(Camera.main.ScreenToWorldPoint(joystickOrigin),
                    Camera.main.ScreenToWorldPoint(Input.mousePosition)) * 75 * Time.deltaTime);

                currentPosition = Vector3.MoveTowards(currentPosition, target, Time.deltaTime * GVar.currentPlant.speed * speedMultiplier);
            }
            else
            {
                currentPosition = Vector3.MoveTowards(currentPosition, target, Time.deltaTime * GVar.currentPlant.speed * speedMultiplier);
            }


            if (Vector3.Distance(previousPosition, currentPosition) > maxDistance)
            {
                //Create new line
                TreeVisualizer.instance.RandomFlower(previousPosition, currentPosition, 0);
                Vector3 prevEndPoint = currentPosition;
                maxDistance = Random.Range(distanceBetweenNodes * 0.75f, distanceBetweenNodes * 1.25f);
                TreeVisualizer.instance.SetNode(currentPosition);
                currentPosition = prevEndPoint;
                previousPosition = prevEndPoint;
                TreeCollisionCheck.AddPosition(prevEndPoint);
                randomPos = Tools.CircularRandom(Vector3.zero, 0.4f);

                nextBranch++;
                if (nextBranch > branchRate)
                {
                    nextBranch = Random.Range(-1,2);
                    //TreeCollisionCheck.AddBranch(prevEndPoint);
                    TreeVisualizer.instance.SetNewLine(prevEndPoint);
                    TreeVisualizer.instance.SetBranch(prevEndPoint);
                }

            }

            //Remove water as you grow
            water = water < 0 ? 0 : water - Time.deltaTime * 2;




            CheckForTargets();
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 6, Time.deltaTime * 3);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 7, Time.deltaTime * 3);
            diggingParticle.Stop();
            Restart();
        }

        speedMultiplier = speedMultiplier > 2 ? speedMultiplier : speedMultiplier + Time.deltaTime * 0.05f;

        cameraFollow.transform.position = Vector3.Lerp(cameraFollow.transform.position, currentPosition, Time.deltaTime * 5);
        diggingParticle.transform.position = currentPosition;

        waterFill.fillAmount = water / maxWater;

        branchHead.transform.position = currentPosition;
        TreeVisualizer.instance.SetPosition(currentPosition);

        if (currentPosition.y < GVar.currentPlant.depth)
        {
            GardenManager.plantGrown = true;
            GameManager.instance.gameEnded = true;
            EffectManager.instance.PlayParticle(EffectManager.instance.rootFinished, currentPosition);
            TreeVisualizer.instance.FinishBranch();
            SoundManager.instance.PlaySound(SoundManager.instance.rootFinish);
            UIManager.instance.WinScreen();
        }
        else
        {
            depthFill.fillAmount = Mathf.Abs(currentPosition.y / GVar.currentPlant.depth); 
        }

        //Visualize reload cooldown
        if (cpReloadTimer < cpCooldown)
        {
            cpReloadTimer += Time.deltaTime;
            cpReloadFill.fillAmount = cpReloadTimer / cpCooldown;
        }


    }

    public Vector3 GetPosition()
    {
        return currentPosition;
    }

    IEnumerator WaitForDoubleTap()
    {
        float t = 0;
        yield return new WaitForSeconds(0.05f);
        while (t < 0.5f)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (altControls)
                {
                    ReloadCheckpoint(true);
                }
                else
                {
                    SwapDirection();
                }
                break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    IEnumerator ResetTrail()
    {
        branchHead.transform.position = currentPosition;
        headTrailEffect = Instantiate(tempTrail, branchHead.transform);
        headTrailEffect.transform.position = branchHead.transform.position;
        headTrailEffect.transform.Translate(Vector3.forward * 0.2f);
        headTrailEffect.startColor = Color.clear;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            headTrailEffect.startColor = Color.Lerp(headTrailEffect.startColor, headTrailColor, Time.deltaTime * 5);
            tempTrail.startColor = Color.Lerp(tempTrail.startColor, Color.clear, Time.deltaTime * 5);
            yield return null;
        }
        Destroy(tempTrail);
        headTrailEffect.startColor = headTrailColor;
        yield return null;
    }

    void SwapDirection()
    {
        if (direction == Vector3.down * 1.5f + Vector3.right)
        {
            direction = Vector3.down * 1.5f - Vector3.right;
        }
        else
        {
            direction = Vector3.down * 1.5f + Vector3.right;
        }
    }

    public void Restart()
    {
        GardenManager.plantGrown = true;
        GardenManager.currentPlant.readyTime = GardenManager.currentPlant.readyTime.AddMinutes(GVar.currentPlant.timeToGrow * (currentPosition.y / GVar.currentPlant.depth));
        GameManager.instance.gameEnded = true;
        UIManager.instance.LoseScreen();
        SoundManager.instance.PlaySound(SoundManager.instance.rootLose);
        Time.timeScale = 1;
        joystickVisual.SetActive(false);

    }

    void DebugText(string text)
    {
        debugText.text = text;
        if (hideTextRoutine != null) StopCoroutine(hideTextRoutine);
        hideTextRoutine = StartCoroutine(HideText(debugText));
    }


    IEnumerator HideText(Text text)
    {

        Color clr = text.color;
        float t = 0;
        while (t < 4f)
        {
            t += Time.unscaledDeltaTime;
            clr.a = (4 - t);
            text.color = clr;
            yield return null;
        }
        yield return null;
    }

    void UpdateUI()
    {
        cpLeftText.text = cpointsleft.ToString();
        cpCountText.text = checkPoints.Count.ToString();
    }

    public void PauseGame(bool pause)
    {
        if (pause) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void ChangeControls()
    {
        altControls = !altControls;
        DebugText("Using alternative controls: " + altControls);
    }


    void CheckForTargets()
    {
        Collider[] hits = Physics.OverlapSphere(currentPosition, 0.1f, interactableLayer);

        if (hits.Length > 0)
        {
            foreach (Collider col in hits)
            {
                if (col.gameObject.tag == "Water")
                {
                    water = water + 40 > 100 ? 100 : water + 25;
                    SoundManager.instance.PlaySound(SoundManager.instance.collectWater);
                    col.gameObject.SetActive(false);
                }
                else if (col.gameObject.tag == "Interactable")
                {
                    col.GetComponent<Interactable>().Interact();
                }

            }
        }
    }

    public void ReloadCheckpoint(bool keepPrevious)
    {
        if (invulnerable)
        {
            return;
        }

        if (cpointsleft < 1 && keepPrevious == false)
        {
            Restart();
            return;
        }
        if (checkPoints.Count < 1 || (cpointsleft < 1)) return;

        if (keepPrevious && cpReloadTimer > cpCooldown)
        {
            cpReloadTimer = 0;
        }
        else if (keepPrevious)
        {
            return;
        }
        else
        {
            invulnerable = true;
            Invoke("ResetInvulnerability", 0.5f);
            cpointsleft--;
        }
        speedMultiplier = 1f;

        Vector3 pos = currentPosition;
        pos = GetPreviousPoint(keepPrevious, pos);
        previousPosition = pos;
        currentPosition = pos;
        TreeVisualizer.instance.SetNewLine(pos);
        TreeCollisionCheck.AddBranch(pos);
        tempTrail = headTrailEffect;
        headTrailEffect.transform.parent = null;
        headTrailEffect = null;
        StartCoroutine(ResetTrail());

        UpdateUI();

    }

    void ResetInvulnerability()
    {
        invulnerable = false;
    }

    Vector3 GetPreviousPoint(bool keepPrevious, Vector3 prev)
    {
        if (checkPoints.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 point = checkPoints[checkPoints.Count - 1];
        checkPoints.RemoveAt(checkPoints.Count - 1);
        if (keepPrevious)
        {
            checkPoints.Insert(0, prev);
        }
        return point;
    }

    public void AddCPPoint()
    {
        cpointsleft++;
        UpdateUI();
    }

    public void AddCheckPoint(Vector3 point)
    {
        if (checkPoints.Count > 5)
        {
            checkPoints.RemoveAt(0);
        }
        checkPoints.Add(point);
    }

    public void DamageNearby()
    {
        Collider[] collisions = Physics.OverlapSphere(currentPosition, GVar.damageRadius, interactableLayer);
        foreach (Collider col in collisions)
        {
            if (col.GetComponent<Interactable>())
            {
                col.GetComponent<Interactable>().Damage();

            }
        }
        StartCoroutine(FixNearbyTiles(currentPosition));
    }

    IEnumerator FixNearbyTiles(Vector3 point)
    {
        yield return new WaitForEndOfFrame();
        Collider[] collisions = Physics.OverlapSphere(point, GVar.damageRadius + 2, interactableLayer);
        foreach (Collider col in collisions)
        {
            if (col.GetComponent<Interactable>())
            {
                MapGen.instance.MakeTile(col.gameObject);
            }
        }
        yield return null;
    }

    IEnumerator TutorialRoot()
    {
        yield return new WaitForSeconds(1);
        // play animation
        pointer.SetActive(true);
        Vector3 offset = new Vector3 (0f, -2f, 0f);
        pointer.transform.position = branchHead.transform.position + offset;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("mouse click");
            // stop animation
            pointer.SetActive(false);
            pointerShown = true;

            yield return new WaitForSeconds(1);
            progressPanel.SetActive(true);

            yield return new WaitForSeconds(2);
            progressPanel.SetActive(false);

            yield return new WaitForSeconds(1);
            waterPanel.SetActive(true);

            yield return new WaitForSeconds(2);
            waterPanel.SetActive(false);

            yield return new WaitForSeconds(1);
            obstaclesPanel.SetActive(true);

            yield return new WaitForSeconds(2);
            obstaclesPanel.SetActive(false);

            yield return new WaitForSeconds(1);
            collectablesPanel.SetActive(true);

            yield return new WaitForSeconds(2);
            collectablesPanel.SetActive(false);

            GardenManager.tutorialStillOn = true;
        }
    }


}
