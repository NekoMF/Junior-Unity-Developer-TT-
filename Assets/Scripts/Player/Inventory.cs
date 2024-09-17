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

    public Dictionary<WeaponData, GameObject> weaponInstances = new Dictionary<WeaponData, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        weaponAmmo = new Dictionary<WeaponData, int>();
    }

    private void Update()
    {
        // Check for the 'Q' key press to switch weapons
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        // Set current weapon
        currentWeapon = newWeapon;

        // Enable only the equipped weapon and disable others
        foreach (var weaponEntry in weaponInstances)
        {
            // Get the Weapon component from the instance
            Weapon weapon = weaponEntry.Value.GetComponent<Weapon>();
            
            // Equip the selected weapon and unequip others
            if (weaponEntry.Key == newWeapon)
            {
                weaponEntry.Value.SetActive(true);
                weapon.Equip(); // Equip this weapon
            }
            else
            {
                weaponEntry.Value.SetActive(false);
                weapon.Unequip(); // Unequip other weapons
            }
        }
    }

    public void SwitchWeapon()
    {
        if (availableWeapons.Count > 0)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % availableWeapons.Count;
            EquipWeapon(availableWeapons[currentWeaponIndex]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Weapon" tag
        if (other.CompareTag("Weapon"))
        {
            // Check if the collided object has a Weapon component
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null && weapon.weaponData != null)
            {
                // Check if the weapon is already in the inventory
                if (!availableWeapons.Contains(weapon.weaponData))
                {
                    // Add the weapon to the inventory
                    AddWeaponToInventory(weapon.weaponData);

                    // Optionally destroy or deactivate the weapon object in the scene
                    Destroy(other.gameObject);
                }
            }
        }
    }

    public void AddWeaponToInventory(WeaponData weaponData)
    {
        // Check if the weapon is already in the inventory
        if (!availableWeapons.Contains(weaponData))
        {
            // Check if there is space available in the inventory (max 3 weapons)
            if (availableWeapons.Count < 3)
            {
                availableWeapons.Add(weaponData);

                // Instantiate the weapon prefab as a child of the player's WeaponHand object
                GameObject weaponPrefab = weaponData.weaponPrefab;
                weaponPrefab.SetActive(false);

                // Determine the correct tag based on the weapon type
                string weaponTag = "";

                switch (weaponData.weaponName)
                {
                    case "AK-47":
                        weaponTag = "Rifle";
                        break;
                    case "BenneliM4":
                        weaponTag = "Shotgun";
                        break;
                    case "M1911":
                        weaponTag = "Pistol";
                        break;
                    default:
                        weaponTag = "WeaponHand"; // Default tag
                        break;
                }

                // Find the correct weapon parent object
                Transform weaponParent = GameObject.FindGameObjectWithTag(weaponTag)?.transform;

                // Instantiate the weapon and set its parent
                GameObject weaponInstance = Instantiate(weaponPrefab, weaponParent);
                // Initialize the ammo for this weapon if not already present
                if (!weaponAmmo.ContainsKey(weaponData))
                {
                    weaponAmmo[weaponData] = weaponData.maxAmmo; // Add weapon with its max ammo
                }

                // Remove the SphereCollider component if it exists
                SphereCollider collider = weaponInstance.GetComponent<SphereCollider>();
                if (collider != null)
                {
                    Destroy(collider);
                }

                weaponInstance.transform.localPosition = Vector3.zero; // Optional: Reset position
                weaponInstance.transform.localRotation = Quaternion.identity; // Optional: Reset rotation

                // Track the instantiated weapon
                weaponInstances[weaponData] = weaponInstance;

                // Equip the newly added weapon if it's the first weapon or the inventory is empty
                if (availableWeapons.Count == 1)
                {
                    EquipWeapon(weaponData);
                    currentWeaponIndex = 0; // Update the index to the new weapon
                }
            }
            else
            {
                Debug.Log("Inventory is full. Cannot add more weapons.");
            }
        }
        else
        {
            Debug.Log("Weapon already in inventory.");
        }
    }
}
