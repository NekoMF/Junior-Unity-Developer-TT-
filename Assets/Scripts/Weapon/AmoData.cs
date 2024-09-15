using UnityEngine;

[CreateAssetMenu(fileName = "AmmoData", menuName = "Inventory/Ammo")]
public class AmmoData : ScriptableObject
{
    public string ammoType;
    public int maxAmmo;
}
