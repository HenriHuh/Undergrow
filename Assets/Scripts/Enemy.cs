using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private string mode;
    public Vector2 walkDirection;
    public GameObject tipFollower;
    public float distance_;
    public float speed;
    public float walking;

    // Start is called before the first frame update
    void Start()
    {
        distance_ = Vector3.Distance(this.transform.position, tipFollower.transform.position);
        walking = speed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (distance_ < 5f)
        {
            mode = "attack"; 
        }
    }

    IEnumerator EnemyMovement()
    {
        switch (mode)
        {
            case "patrol":
                //move the enemy from left to right
                Debug.Log("patrolling");
                yield return null;
                        
                break;

            case "attack":

                transform.position = Vector3.MoveTowards(transform.position, tipFollower.transform.position, walking);
                Debug.Log("attacking");
                yield return null;

                break;

        }
    }
}
