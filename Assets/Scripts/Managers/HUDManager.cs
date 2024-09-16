using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        // Update the HUD elements based on the current weapon in the inventory
        WeaponData currentWeapon = Inventory.Instance.currentWeapon;

        if (currentWeapon != null)
        {
            // Display current weapon's ammo info
            Weapon activeWeapon = Inventory.Instance.weaponInstances[currentWeapon].GetComponent<Weapon>();

            if (activeWeapon != null && activeWeapon.isEquipped)
            {
                // Update the magazine ammo UI (bullets left in the current magazine)
                magazineAmmoUI.text = activeWeapon.magazineBulletsLeft.ToString();

                // Update the total ammo UI (total bullets available for the weapon)
                int totalAmmo = Inventory.Instance.weaponAmmo.ContainsKey(currentWeapon)
                    ? Inventory.Instance.weaponAmmo[currentWeapon]
                    : 0; // Fallback to 0 if the weapon is not in the dictionary
                totalAmmoUI.text = totalAmmo.ToString();

                // Update the active weapon UI to display the current weapon's sprite
                activeWeaponUI.sprite = currentWeapon.weaponSprite;

                // Optional: Update the ammo type image if you have different ammo types for different weapons
                // ammoType.sprite = currentWeapon.ammoTypeSprite; // Uncomment this if you have an ammo type sprite
            }
        }
    }
}
