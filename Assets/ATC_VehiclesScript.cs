using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ATC_Vehicle : MonoBehaviour
{
    public Transform destination;
    //public float speed;
    NavMeshAgent agent;
    NavMeshPath path;
    Vector3 prevTarget;
    Vector3 currentDir;
    //Vector3 currentTarget;
    //int cornerIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        prevTarget = transform.position;
        path = new NavMeshPath();   
        agent = GetComponent<NavMeshAgent>();
        agent.CalculatePath(destination.position, path);
        Debug.Log("Destination Point: " + destination.position);
    }

    // Update is called once per frame
    void Update()
    {

        agent.destination = destination.position;
    }

    void FixPath()
    {
        
        if (agent.isStopped) return;
        foreach (var p in path.corners)
        {
            Debug.Log("Previous pos: " + prevTarget);
            Debug.Log("Waypoint: " + p);
            var rawDir = p- prevTarget ;
            var distance = rawDir.magnitude;
            if(distance <1f) continue;
            currentDir = new Vector3(Mathf.Round((rawDir.normalized).x), 0f, Mathf.Round((rawDir.normalized).z));
            prevTarget = p;
            
            //agent.destination = fixDir * distance;
        }
    }

    //void Move()
    //{
    //    if (agent.isStopped) return;
    //    agent.velocity = currentDir * agent.speed;
    //    if (Vector3.Distance(transform.position, destination.position) < 2f)
    //    {
    //        agent.velocity = Vector3.zero;
    //        agent.isStopped = true;
    //    }
    //    if (Vector3.Distance(transform.position, currentTarget) < 2f)
    //    {
    //        cornerIndex++;
    //        GetNextDestination(cornerIndex)
    //    }
    //}

    //void GetNextDestination(int index)
    //{
    //    currentTarget = path.corners[index];
    //    Debug.Log("Previous pos: " + prevTarget);
    //    Debug.Log("Waypoint: " + currentTarget);
    //    var rawDir = currentTarget - prevTarget;
    //    var distance = rawDir.magnitude;
    //    if (distance > 1f)
    //    {
    //        currentDir = new Vector3(Mathf.Round((rawDir.normalized).x), 0f, Mathf.Round((rawDir.normalized).z));
    //        prevTarget = currentTarget;
    //    }
    //    Debug.Log(currentDir);
    //}
}
