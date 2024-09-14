using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float playerHP = 100f;

    public void TakeDamage (int damageAmount)
    {
        playerHP -= damageAmount;
        
        if (playerHP <= 0)
        {
           print("DEATH");
        }
        else
        {
            print("Player Hit");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("ZombieHand"))
        {
            TakeDamage(25);
        }
    }
}
