using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    [SerializeField] private int zombieHP = 100;

    private Animator animator;
    public bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
            isDead = true;
        }
        else
        {
            animator.SetTrigger("DAMAGE");
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f);     

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f);   

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f);   
    }
}
