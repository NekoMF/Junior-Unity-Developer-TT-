using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChasingState : StateMachineBehaviour
{

    Transform player;
    NavMeshAgent navMeshAgent;
    ZombieData zombieData;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        zombieData = animator.GetComponent<Zombie>()?.GetZombieData();
        player = GameObject.FindWithTag("Player").transform;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = zombieData.chaseSpeed; 
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navMeshAgent.SetDestination(player.position);
       animator.transform.LookAt(player);

       float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
       if (distanceFromPlayer>zombieData.stopChasingDistance)
       {
            animator.SetBool("isChasing", false);
       }

       if (distanceFromPlayer < zombieData.attackDistance)
       {
            animator.SetBool("isAttacking", true);
       }
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navMeshAgent.SetDestination (animator.transform.position);
    }
}
