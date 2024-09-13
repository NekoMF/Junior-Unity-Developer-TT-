using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    [SerializeField] private int zombieHP = 100;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void TakeDamage (int damageAmount)
    {
        zombieHP -= damageAmount;
        
        if (zombieHP <= 0)
        {
            animator.SetTrigger(UnityEngine.Random.Range(0, 2) == 0 ? "DIE1" : "DIE2");
            
            // Disable the collider
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }
}
