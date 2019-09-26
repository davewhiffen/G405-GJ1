using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingEnemy : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private Transform targetPoint;
    private int wpIndex = 0;
    private float minDistance = 0.5f;
    private float minRotation = 0.1f;
    private int lastWpIndex;
    private bool increaseIndex = true;
    private bool isPatrolling = true;
    private bool isLooking = false;
    private bool isChasing = false;
    private bool setRotation = false;
    private Quaternion tempRot;
    private int rotCount = 0;
    private FieldOfView FOV;
    private Transform targetPlayer;
    private NavMeshAgent agent;
    private GameManager manager;

    private bool isAlerted = false;
    private bool lostTarget = false;

    public float moveSpeed = 3.0f;
    public float rotSpeed = 2.0f;

    public GameObject testObj;


    enum EnemyState
    {
        Patrol,
        Chase,
        Look
    };

    // Start is called before the first frame update
    void Start()
    {
        FOV = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();
        manager = FindObjectOfType<GameManager>();

        lastWpIndex = waypoints.Count - 1;
        targetPoint = waypoints[wpIndex];
    }

    IEnumerator Patrol()
    {
        //float moveStep = moveSpeed * Time.deltaTime;
        //float rotStep = rotSpeed * Time.deltaTime;

        //Vector3 dirToTarget = targetPoint.position - transform.position;
        //Quaternion rotToTarget = Quaternion.LookRotation(dirToTarget);

        ////transform.rotation = rotToTarget;
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotToTarget, rotStep);

        //float distance = Vector3.Distance(transform.position, targetPoint.position);
        ////Debug.Log("Distance: " + distance);
        //CheckDistanceToWaypoint(distance);
        //transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveStep);

        //float distance = Vector3.Distance(transform.position, targetPoint.position);
        //CheckDistanceToWaypoint(distance);

        CheckDistanceToWaypoint();
        agent.SetDestination(targetPoint.position);

        yield return null;
    }

    IEnumerator Chase()
    {
        if(isAlerted)
        {
            isAlerted = false;
            yield return new WaitForSeconds(0.1f);            
            agent.isStopped = false;
            agent.angularSpeed = 120;
        }

        
            agent.SetDestination(targetPlayer.position);

        yield return null;
    }

    IEnumerator LostTarget()
    {
        yield return new WaitForSeconds(1.5f);
        agent.angularSpeed = 0;
        agent.isStopped = true;
        agent.speed = 2.5f;
        isChasing = false;
        yield return new WaitForSeconds(0.5f);        
        isLooking = true;
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
        if (isChasing)
        {
            StartCoroutine(Chase());            
        }

        if(FOV.visibleTargets.Any(i => i != null) && !isChasing)
        {
            int val = FOV.visibleTargets.Count;
            targetPlayer = FOV.visibleTargets[val - 1];

            if(targetPlayer.gameObject.GetComponent<PlayerController>().Hiding == false)
            {
                isChasing = true;
                isAlerted = true;
                StopAllCoroutines();
                isPatrolling = false;
                isLooking = false;
                agent.speed = 5.5f;
            }

            //Debug.Log("Target Acquired");
        }
        else
        {
            if ((FOV.visibleTargets.Count == 0) && isChasing && !lostTarget)
            {
                lostTarget = true;
                StartCoroutine(LostTarget());
                //Debug.Log("Target Lost");
            }
        }

    }

    void CheckDistanceToWaypoint()
    {
        //if (currDist <= minDistance)
        if (Vector3.Distance(targetPoint.transform.position, transform.position) < 0.5f)
        {
            isPatrolling = false;
            agent.isStopped = true;
            StopAllCoroutines();
            UpdateTargetWaypoint();
            agent.angularSpeed = 0;
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
                agent.angularSpeed = 120;
                agent.isStopped = false;
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
        if (increaseIndex)
        {
            wpIndex++;
        }
        else if(!increaseIndex)
        {
            wpIndex--;
        }

        if (wpIndex > lastWpIndex)
        {
            increaseIndex = false;
            wpIndex = wpIndex - 2;
        }
        if(wpIndex < 0)
        {
            increaseIndex = true;
            wpIndex = wpIndex + 2;
        }
        targetPoint = waypoints[wpIndex];
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Player")
    //    {
    //        var manager = FindObjectOfType<GameManager>();
    //        manager.playerDead++;
    //        Destroy(collision.gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            manager.playerDead++;
            StopAllCoroutines();
            isChasing = false;
            isPatrolling = true;
            agent.isStopped = true;
            agent.speed = 2.5f;
            Destroy(other.gameObject, 0.2f);
        }
    }
}
