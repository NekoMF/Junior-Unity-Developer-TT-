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

        if (weaponData == null) return; // Do nothing if weaponData is null

        if (magazineBulletsLeft == 0 && isShooting)
        {
            if (!SoundManger.Instance.emptyMagazineSound.isPlaying) // Check if the sound is already playing
            {
                SoundManger.Instance.emptyMagazineSound.Play();
            }
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

        if ((Input.GetKeyDown(KeyCode.R) && magazineBulletsLeft < weaponData.magazineSize) || (readyToShoot && !isShooting && !isReloading && magazineBulletsLeft <= 0))
        {
            Reload();
        }

        if (readyToShoot && isShooting && magazineBulletsLeft > 0)
        {
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{magazineBulletsLeft}/{weaponData.magazineSize}";
        }
    }

    private void FireWeapon()
    {

        magazineBulletsLeft--;

        if (muzzleEffect != null)
        {
            muzzleEffect.GetComponent<ParticleSystem>().Play();
        }

        if (SoundManger.Instance.shootingSoundAK47 != null)
        {
            SoundManger.Instance.shootingSoundAK47.Play();
        }

        readyToShoot = false;

        // Calculate random spread
        Vector3 spread = new Vector3(
            UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity),
            UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity),
            UnityEngine.Random.Range(-weaponData.spreadIntensity, weaponData.spreadIntensity)
        );

        // Create ray direction with spread
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Center of the screen
        RaycastHit hit;

        // Apply spread to the ray direction
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
                Debug.Log("zombie head");

                Zombie zombie = hit.collider.GetComponentInParent<Zombie>();

                if (zombie != null)
                {
                    int headshotDamage = (int)(weaponData.damage * 2); // Apply headshot damage
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
        isReloading = true;
        Invoke("ReloadCompleted", weaponData.reloadTime);
        if (SoundManger.Instance.reloadingSoundAK47 != null)
        {
            SoundManger.Instance.reloadingSoundAK47.Play();
        }
    }

    private void ReloadCompleted()
    {
        magazineBulletsLeft = weaponData.magazineSize;
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
