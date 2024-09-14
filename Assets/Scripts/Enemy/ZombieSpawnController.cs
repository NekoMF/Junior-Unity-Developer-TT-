using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiePerWave = 5;
    public int currentZombiePerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;
    public float waveCooldown = 10f; 

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Zombie> currentZombiesAlive;

    public GameObject zombiePrefab; 

    private void Start()
    {
        currentZombiePerWave = initialZombiePerWave; 
        GlobalReferences.Instance.waveNumber = currentWave;
        StartNextWave();      
    }

    private void Update()
    {
        List<Zombie> zombiesToRemove = new List<Zombie>();
        foreach (Zombie zombie in currentZombiesAlive)
        {
            if (zombie == null || zombie.isDead) // Check if the zombie is null or dead
            {
                zombiesToRemove.Add(zombie);
            }
        }

        foreach (Zombie zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();

        if (currentZombiesAlive.Count == 0 && !inCooldown)
        {
            StartCoroutine(WaveCooldown());
        }

        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }
    }


    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        yield return new WaitForSeconds (waveCooldown);
        inCooldown = false;
        currentZombiePerWave *=2 ;
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();
        currentWave ++;
        StartCoroutine(SpawnWave());
        GlobalReferences.Instance.waveNumber = currentWave;
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiePerWave; i++)
        {
            Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-1f, 1), 0f, UnityEngine.Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Zombie zombieScript = zombie.GetComponent<Zombie>();

            currentZombiesAlive.Add(zombieScript);

            yield return new WaitForSeconds(spawnDelay); 
        }
    }
}
