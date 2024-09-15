using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "ScriptableObjects/ZombieData", order = 1)]
public class ZombieData : ScriptableObject
{
    public string zombieName;
    public int maxHP;
    public int damage;
    public float chaseSpeed;
    public float patrollingSpeed;
    public float detectionAreaRadius = 18f;
    public float attackDistance = 2.5f;
    public float stopChasingDistance = 21f;
    public float patrollingTime = 15f;
    public float idleTime = 2f;
    public GameObject zombiePrefab;
}
