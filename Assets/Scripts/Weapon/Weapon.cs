using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Reference to WeaponData
    public WeaponData weaponData;

    // UI
    public TextMeshProUGUI ammoDisplay;

    // Shooting
    public bool isEquipped = false;

    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public bool isReloading;
    public int magazineBulletsLeft; // Current number of bullets in the magazine
    private bool hasPlayedEmptySound = false; // Flag to track empty magazine sound

    // Bullet
    public GameObject muzzleEffect;

    private void Awake()
    {
        readyToShoot = true;
    }

    private void Start()
    {
        if (weaponData != null)
        {
            magazineBulletsLeft = weaponData.magazineSize; // Initialize magazine with full bullets
        }
    }

    void Update()
    {
        if (!isEquipped || weaponData == null) return; // Do nothing if weapon is not equipped

        // Handle empty magazine sound
        if (magazineBulletsLeft == 0 && isShooting)
        {
            if (!hasPlayedEmptySound)
            {
                AudioManager.Instance.PlaySFX("Magazine Empty", 1f);
                hasPlayedEmptySound = true; // Set flag so sound plays only once
            }
        }
        else
        {
            // Reset the empty sound flag if there are bullets available
            hasPlayedEmptySound = false;
        }

        // Determine shooting mode based on the int value from WeaponData
        bool isAuto = weaponData.shootingMode == 2;
        bool isSingle = weaponData.shootingMode == 1;

        if (isAuto)
        {
            // Holding the Key
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (isSingle)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        // Reload only if there is ammo available
        if ((Input.GetKeyDown(KeyCode.R) && magazineBulletsLeft < weaponData.magazineSize && Inventory.Instance.weaponAmmo.ContainsKey(weaponData) && Inventory.Instance.weaponAmmo[weaponData] > 0) ||
            (readyToShoot && !isShooting && !isReloading && magazineBulletsLeft <= 0 && Inventory.Instance.weaponAmmo.ContainsKey(weaponData) && Inventory.Instance.weaponAmmo[weaponData] > 0))
        {
            Reload();
        }

        if (readyToShoot && isShooting && magazineBulletsLeft > 0)
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        magazineBulletsLeft--;

        if (muzzleEffect != null)
        {
            muzzleEffect.GetComponent<ParticleSystem>().Play();
        }

        string soundToPlay = "";
        switch (weaponData.weaponName)
        {
            case "M1911":
                soundToPlay = "Pistol Fire";
                break;
            case "AK-47":
                soundToPlay = "Rifle Fire";
                break;
            case "BenneliM4":
                soundToPlay = "Shotgun Fire";
                break;
            default:
                Debug.LogWarning("No specific sound assigned for this weapon.");
                break;
        }

        if (!string.IsNullOrEmpty(soundToPlay))
        {
            AudioManager.Instance.PlaySFX(soundToPlay, 1.0f);
        }

        readyToShoot = false;

        Vector3 spread = new Vector3(
            UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity),
            UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity),
            UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity)
        );

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Vector3 direction = ray.direction + spread;
        Ray spreadRay = new Ray(ray.origin, direction);

        if (Physics.Raycast(spreadRay, out hit, weaponData.range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (!(hit.collider.CompareTag("Zombie") || hit.collider.CompareTag("ZombieHead")))
            {
                CreateBulletImpactEffect(hit);
            }

            if (hit.collider.CompareTag("ZombieHead"))
            {
                Zombie zombie = hit.collider.GetComponentInParent<Zombie>();

                if (zombie != null)
                {
                    int headshotDamage = (int)(weaponData.damage * 2);
                    zombie.TakeDamage(headshotDamage);
                    CreateDamageIndicatorEffect(hit, headshotDamage);
                }

                CreateBloodSplashEffect(hit);
            }
            else if (hit.collider.CompareTag("Zombie"))
            {
                Zombie zombie = hit.collider.GetComponent<Zombie>();

                if (zombie != null)
                {
                    zombie.TakeDamage((int)weaponData.damage);
                    CreateDamageIndicatorEffect(hit, (int)weaponData.damage);
                }

                CreateBloodSplashEffect(hit);
            }
        }

        if (allowReset)
        {
            Invoke("ResetShot", weaponData.shootingDelay);
            allowReset = false;
        }
    }

    private void Reload()
    {
        // Only reload if there is ammo available in the inventory
        if (Inventory.Instance.weaponAmmo.ContainsKey(weaponData) && Inventory.Instance.weaponAmmo[weaponData] > 0)
        {
            isReloading = true;
            Invoke("ReloadCompleted", weaponData.reloadTime);
            AudioManager.Instance.PlaySFX("Reload", 1.0f);
        }
    }

    private void ReloadCompleted()
    {
        int totalAmmo = Inventory.Instance.weaponAmmo.ContainsKey(weaponData) ? Inventory.Instance.weaponAmmo[weaponData] : 0;
        int ammoNeeded = weaponData.magazineSize - magazineBulletsLeft;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);

        Inventory.Instance.weaponAmmo[weaponData] -= ammoToReload;
        magazineBulletsLeft += ammoToReload;

        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    void CreateBulletImpactEffect(RaycastHit hitInfo)
    {
        if (GlobalReferences.Instance.bulletImpactEffectPrefab != null)
        {
            Instantiate(
                GlobalReferences.Instance.bulletImpactEffectPrefab,
                hitInfo.point,
                Quaternion.LookRotation(hitInfo.normal)
            );
        }
    }

    void CreateBloodSplashEffect(RaycastHit hitInfo)
    {
        if (GlobalReferences.Instance.bloodSprayEffectPrefab != null)
        {
            Instantiate(
                GlobalReferences.Instance.bloodSprayEffectPrefab,
                hitInfo.point,
                Quaternion.LookRotation(hitInfo.normal)
            );
        }
    }

    void CreateDamageIndicatorEffect(RaycastHit hitInfo, int damageAmount)
    {
        if (GlobalReferences.Instance.damageIndicatorEffectPrefab != null)
        {
            GameObject indicator = Instantiate(
                GlobalReferences.Instance.damageIndicatorEffectPrefab,
                hitInfo.point,  
                Quaternion.identity  
            );

            var textMesh = indicator.GetComponent<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = damageAmount.ToString();
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                textMesh.transform.LookAt(mainCamera.transform);
                textMesh.transform.Rotate(0, 180, 0);
            }
        }
    }

    // You can also add a method to switch weapon data dynamically
    public void UpdateWeaponData(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
        magazineBulletsLeft = weaponData.magazineSize; // Reset the bullets count to full magazine
    }

    public void Equip()
    {
        isEquipped = true;
    }

    // Unequip this weapon
    public void Unequip()
    {
        isEquipped = false;
    }
}
