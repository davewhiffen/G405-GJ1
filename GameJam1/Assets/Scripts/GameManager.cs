using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> playerList = new List<GameObject>();
    public List<GameObject> objectivesList = new List<GameObject>();
    public Text objDesc;

    public int objectiveTimer = 0;
    private int currObjectiveValue = 0;

    private int objIndex = 0;
    private int lastObjIndex = 0;
    private GameObject currObjective;
    private bool objectiveInProgress = false;
    private int playerAmnt = 0;
    [HideInInspector] public int objCompleted = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerList.Add(obj);
        }
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Objective"))
        {
            objectivesList.Add(obj);
        }
        lastObjIndex = objectivesList.Count - 1;
        playerAmnt = playerList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(!objectiveInProgress)
        {
            objectiveInProgress = true;
            StartNewObjective();
            StartCoroutine(ObjectiveCountdown(objectiveTimer));
        }

        if(objCompleted == playerAmnt)
        {
            objCompleted = 0;
            Coroutine co = StartCoroutine(ObjectiveCountdown(0));
            StopCoroutine(co);
            EndCurrentObjective();
        }
    }

    IEnumerator ObjectiveCountdown(int countdownValue)
    {        
        currObjectiveValue = countdownValue;
        while (currObjectiveValue > 0)
        {
            Debug.Log("Time: " + currObjectiveValue);
            yield return new WaitForSeconds(1.0f);
            currObjectiveValue--;
        }
        Debug.Log("Timer ended");

        EndCurrentObjective();
        yield return new WaitForSeconds(2.0f);
        objectiveInProgress = false;
        yield return null;
    }

    void StartNewObjective()
    {
        currObjective = objectivesList[objIndex];
        currObjective.GetComponent<OutlineTest>().enabled = true;
        var objective = currObjective.GetComponent<ObjectiveObject>();
        objective.objectiveEnabled = true;
        objDesc.text = objective.description;
    }

    void EndCurrentObjective()
    {
        var outline = currObjective.GetComponent<OutlineTest>();
        outline.rend.material.SetFloat("_OutlineWidth", 1);
        outline.enabled = false;
        currObjective.GetComponent<ObjectiveObject>().objectiveEnabled = false;
        objIndex++;
        if(objIndex > lastObjIndex)
        {
            objIndex = 0;
        }

        foreach(GameObject obj in playerList)
        {
            obj.GetComponent<PlayerController>().completedObjective = false;
        }
    }
}
