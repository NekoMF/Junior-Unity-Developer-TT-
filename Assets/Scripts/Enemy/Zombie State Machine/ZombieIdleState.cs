using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ZombieIdleState : StateMachineBehaviour
{

    float timer;

    Transform player; 
    ZombieData zombieData;
    Zombie zombie;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       zombieData = animator.GetComponent<Zombie>()?.GetZombieData();
       timer = 0;
       player = GameObject.FindWithTag("Player").transform;
       zombie = animator.GetComponent<Zombie>();
       
       
       
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       timer += Time.deltaTime;
       if (timer > zombieData.idleTime)
       {
            animator.SetBool("isPatrolling", true);
       }

       float distanceFromPlayer = Vector3.Distance (player.position, animator.transform.position);
       if (distanceFromPlayer < zombieData.detectionAreaRadius)
       {
         animator.SetBool("isChasing", true);
       }
    }


}
