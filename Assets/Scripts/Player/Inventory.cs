using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Singleton Instance
    public static Inventory Instance { get; private set; }

    public List<WeaponData> availableWeapons; // Max 3 weapons
    public WeaponData currentWeapon;
    public int currentWeaponIndex = 0;
    public Dictionary<WeaponData, int> weaponAmmo; // Track ammo for each weapon

    private void Awake()
    {
        // Singleton Pattern Implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this instance alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if they exist
        }
    }

    private void Start()
    {
        // Initialize ammo for each weapon
        weaponAmmo = new Dictionary<WeaponData, int>();

        foreach (WeaponData weapon in availableWeapons)
        {
            weaponAmmo[weapon] = weapon.maxAmmo; // Set initial ammo
        }

        if (availableWeapons.Count > 0)
        {
            EquipWeapon(availableWeapons[currentWeaponIndex]);
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        // Instantiate the weapon's prefab or enable the weapon
        // Update the player's weapon-related stats like damage, reload time, etc.
    }

    public void SwitchWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % availableWeapons.Count;
        EquipWeapon(availableWeapons[currentWeaponIndex]);
    }
}
