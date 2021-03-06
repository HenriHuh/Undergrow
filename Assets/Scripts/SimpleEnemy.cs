﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    float timeToRoam = 4;
    float timer = 0;
    float direction = 1;
    public GameObject lookoutSphere;
    public float speed;
    public float attacking;

    float timeFromTurn;
    int turnCount;

    public static List<GameObject> moles = new List<GameObject>();

    void Start()
    {
        if (Random.Range(0,2) == 0)
        {
            gameObject.SetActive(false);
        }
        timer = timeToRoam / 2;
        direction = (int)Mathf.Sign(Random.Range(-1, 1));
        Vector3 localScale = transform.GetChild(0).transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * -direction;
        transform.GetChild(0).transform.localScale = localScale;
        attacking = speed * Time.deltaTime / 2;
    }

    private void OnEnable()
    {
        moles.Add(gameObject);
        timeFromTurn = 1f;
        turnCount = 0;
    }

    private void OnDisable()
    {
        moles.Remove(gameObject);
        EffectManager.instance.PlayParticle(EffectManager.instance.moleDestroy, transform.position);
    }

    void Update()
    {
        if (GameManager.instance.gameEnded) return;

        timer += Time.deltaTime;
        if (timer > timeToRoam)
        {
            //Direction change
            direction = direction * -1f;
            timer = 0;
            Vector3 localScale = transform.GetChild(0).transform.localScale;
            localScale.x = Mathf.Abs(localScale.x)  * -direction;
            transform.GetChild(0).transform.localScale = localScale;
        }

        transform.Translate(Vector3.right * direction * Time.deltaTime);

        if (TreeCollisionCheck.CheckCurrentCollision(DrawTree.instance.interactableLayer, gameObject))
        {
            DrawTree.instance.ReloadCheckpoint(false);
            SoundManager.instance.PlaySound(SoundManager.instance.hitMole);
            gameObject.SetActive(false);
        }

        timeFromTurn += Time.deltaTime;
        if (Physics.Linecast(transform.position, transform.position + (transform.right * direction), GameManager.instance.obstacleLayer))
        {
            direction = direction * -1f;
            timer = 0;
            Vector3 localScale = transform.GetChild(0).transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * -direction;
            transform.GetChild(0).transform.localScale = localScale;

            //Disable mole if it's stuck
            if (timeFromTurn < 0.25f)
            {
                turnCount++;
                if (turnCount > 3) gameObject.SetActive(false);
            }
            timeFromTurn = 0;
        }

        if(TreeCollisionCheck.CheckCurrentCollision(DrawTree.instance.interactableLayer, lookoutSphere))
        {
            transform.position = Vector3.MoveTowards(transform.position, DrawTree.instance.GetPosition() + Vector3.up, attacking);
            timer = 0;
            var rotationVector = transform.rotation.eulerAngles;
            Vector3 localScale = transform.GetChild(0).transform.localScale;
            if (localScale.x < 0)
            {
                rotationVector.z = 40f;
                transform.rotation = Quaternion.Euler(rotationVector);
            }
            if (localScale.x > 0)
            {
                rotationVector.z = -40f;
                transform.rotation = Quaternion.Euler(rotationVector);
            }
        }
        transform.Translate(Vector3.right * direction * Time.deltaTime);
    }
}
