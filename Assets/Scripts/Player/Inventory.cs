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

    private Dictionary<WeaponData, GameObject> weaponInstances = new Dictionary<WeaponData, GameObject>();

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
        currentWeapon = newWeapon;

        // Enable only the equipped weapon and disable others
        foreach (var weaponEntry in weaponInstances)
        {
            weaponEntry.Value.SetActive(weaponEntry.Key == newWeapon);
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
            // Check if the collided object has a WeaponTest component
            WeaponTest weaponTest = other.GetComponent<WeaponTest>();
            if (weaponTest != null && weaponTest.weaponData != null)
            {
                // Check if the weapon is already in the inventory
                if (!availableWeapons.Contains(weaponTest.weaponData))
                {
                    // Add the weapon to the inventory
                    AddWeaponToInventory(weaponTest.weaponData);

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
