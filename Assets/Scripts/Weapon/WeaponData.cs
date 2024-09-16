using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Inventory/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float range;
    public int magazineSize;
    public float spreadIntensity;
    public float reloadTime;
    public float shootingDelay;
    public GameObject weaponPrefab;
    public int maxAmmo;
    public int shootingMode;
    
}
