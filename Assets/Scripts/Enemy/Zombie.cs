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
            // Play one of the death animations randomly
            animator.SetTrigger(UnityEngine.Random.Range(0, 2) == 0 ? "DIE1" : "DIE2");

            // Disable all colliders in this object, including children
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            isDead = true;

            // Destroy the zombie game object after 7 seconds
            Destroy(gameObject, 7f);
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
