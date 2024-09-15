using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "ScriptableObjects/ZombieData", order = 1)]
public class ZombieData : ScriptableObject
{
    public int maxHP = 100;
    public int damage = 10;
    public float speed = 3f;
}
