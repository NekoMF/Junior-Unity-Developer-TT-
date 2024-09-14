using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : StateMachineBehaviour
{

    Transform player;
    NavMeshAgent navMeshAgent;

    float stopAttackingDistance = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindWithTag("Player").transform;
        navMeshAgent = animator.GetComponent<NavMeshAgent>();
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();    

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer>stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - navMeshAgent.transform.position;
        navMeshAgent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = navMeshAgent.transform.eulerAngles.y;
        navMeshAgent.transform.rotation = Quaternion.Euler(0, yRotation, 0);    
    }
}
