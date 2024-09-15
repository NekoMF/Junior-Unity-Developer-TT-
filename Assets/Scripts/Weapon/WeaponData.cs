using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Inventory/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float range;
    public int magazineSize;
    public float reloadTime;
    public GameObject weaponPrefab;
    public int maxAmmo;
    public ShootingMode shootingMode;
    public enum ShootingMode
    {
        Single,
        Auto
    }
}
