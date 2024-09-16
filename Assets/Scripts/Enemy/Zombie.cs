using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] private ZombieData zombieData;

    private int currentHP;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    public bool isDead;
    private bool hasPlayedHitSound = false; // Flag to track hit sound

    private void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentHP = zombieData.maxHP;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;

        if (currentHP <= 0)
        {
            // Play one of the death animations randomly
            animator.SetTrigger(UnityEngine.Random.Range(0, 2) == 0 ? "DIE1" : "DIE2");
            AudioManager.Instance.PlaySFX("Zombie Death", 0.5f);

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
            // Play hit sound only if not already played
            if (!hasPlayedHitSound)
            {
                AudioManager.Instance.PlaySFX("Zombie Hit", 0.5f);
                hasPlayedHitSound = true; // Set flag to true after playing sound
                StartCoroutine(ResetHitSoundFlag()); // Start coroutine to reset flag
            }

            animator.SetTrigger("DAMAGE");
        }
    }

    private IEnumerator ResetHitSoundFlag()
    {
        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds
        hasPlayedHitSound = false; // Reset the flag
    }

    public int GetDamage()
    {
        return zombieData.damage;
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

    public void SetZombieData(ZombieData data)
    {
        zombieData = data;
    }

    public ZombieData GetZombieData()
    {
        return zombieData;
    }
}
