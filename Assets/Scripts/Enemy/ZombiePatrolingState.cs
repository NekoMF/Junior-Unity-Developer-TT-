using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrolingTime;

    Transform player;
    NavMeshAgent navMeshAgent;

    public float detectionAreaRadius = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindWithTag("Player").transform;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = patrolSpeed;


        GameObject waypointCluster = GameObject.FindWithTag("Waypoints");
        foreach (Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);            
        }

        Vector3 nextPosition = waypointsList[Random.Range (0, waypointsList.Count)].position;
        navMeshAgent.SetDestination(nextPosition);
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
       {
            navMeshAgent.SetDestination(waypointsList[Random.Range (0, waypointsList.Count)].position);
       }

       timer += Time.deltaTime;
       if (timer > patrolingTime)
       {
            animator.SetBool("isPatroling", false);
       }

       float distanceFromPlayer = Vector3.Distance (player.position, animator.transform.position);
       if (distanceFromPlayer < detectionAreaRadius)
       {
            animator.SetBool("isChasing", true);
       }

    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navMeshAgent.SetDestination(navMeshAgent.transform.position);
    }
}
