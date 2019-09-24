using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private Transform targetPoint;
    private int wpIndex = 0;
    private float minDistance = 0.1f;
    private float minRotation = 0.1f;
    private int lastWpIndex;
    private bool isPatrolling = true;
    private bool isLooking = false;
    private bool setRotation = false;
    private Quaternion tempRot;
    private int rotCount = 0;

    public float moveSpeed = 3.0f;
    public float rotSpeed = 2.0f;


    enum EnemyState
    {
        Patrol,
        Chase,
        Look
    };

    // Start is called before the first frame update
    void Start()
    {
        lastWpIndex = waypoints.Count - 1;
        targetPoint = waypoints[wpIndex];
    }

    IEnumerator Patrol()
    {
        float moveStep = moveSpeed * Time.deltaTime;
        float rotStep = rotSpeed * Time.deltaTime;

        Vector3 dirToTarget = targetPoint.position - transform.position;
        Quaternion rotToTarget = Quaternion.LookRotation(dirToTarget);

        //transform.rotation = rotToTarget;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToTarget, rotStep);

        float distance = Vector3.Distance(transform.position, targetPoint.position);
        //Debug.Log("Distance: " + distance);
        CheckDistanceToWaypoint(distance);

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveStep);

        yield return null;
    }

    IEnumerator Chase()
    {
        yield return null;
    }

    IEnumerator Look()
    {
        float rotStep = rotSpeed * Time.deltaTime * 50f;

        if (setRotation)
        {
            setRotation = false;
            tempRot = transform.rotation * Quaternion.Euler(0, 90, 0);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tempRot, rotStep);
        StartCoroutine(CheckRotation(tempRot));


        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPatrolling)
        {
            StartCoroutine(Patrol());
        }
        if (isLooking)
        {
            StartCoroutine(Look());
        }

    }

    void CheckDistanceToWaypoint(float currDist)
    {
        if (currDist <= minDistance)
        {
            isPatrolling = false;
            UpdateTargetWaypoint();
            isLooking = true;
            setRotation = true;
        }
    }

    IEnumerator CheckRotation(Quaternion finalRot)
    {
        if (transform.rotation == finalRot)
        {
            isLooking = false;
            yield return new WaitForSeconds(0.5f);

            if (rotCount > 2)
            {
                rotCount = 0;
                isPatrolling = true;
            }
            else
            {
                rotCount++;
                isLooking = true;
                setRotation = true;
            }
        }

        yield return null;
    }

    void UpdateTargetWaypoint()
    {
        wpIndex++;
        if (wpIndex > lastWpIndex)
        {
            wpIndex = 0;
        }
        targetPoint = waypoints[wpIndex];
    }
}
