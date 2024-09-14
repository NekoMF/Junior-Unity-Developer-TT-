using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChasingState : StateMachineBehaviour
{

    Transform player;
    NavMeshAgent navMeshAgent;

    public float chaseSpeed = 6f;
    public float stopChasingDistance = 21f;
    public float attackDistance = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindWithTag("Player").transform;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();

        navMeshAgent.speed = chaseSpeed;
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navMeshAgent.SetDestination(player.position);
       animator.transform.LookAt(player);

       float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
       if (distanceFromPlayer>stopChasingDistance)
       {
            animator.SetBool("isChasing", false);
       }

       if (distanceFromPlayer < attackDistance)
       {
            animator.SetBool("isAttacking", true);
       }
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navMeshAgent.SetDestination (animator.transform.position);
    }
}
