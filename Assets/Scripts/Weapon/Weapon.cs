using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    //UI
    public TextMeshProUGUI ammoDisplay;

    //Shooting
    public float range = 1000f;
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;
    public float weaponDamage = 7;

    public float reloadTime;
    public int magazineSize, magazineBulletsLeft;
    public bool isReloading;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft; 

    //Spread
    public float spreadIntensity;

    //Bullet
    public GameObject muzzleEffect;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    } 

    public ShootingMode currentShootingMode; 

    private void Awake() 
    {
        readyToShoot = true;    
        burstBulletsLeft = bulletsPerBurst;
        magazineBulletsLeft = magazineSize;
    }

    private void Start() 
    {
        
    }
   

    void Update()
    {

        if (magazineBulletsLeft == 0 && isShooting)
    {
        if (!SoundManger.Instance.emptyMagazineSound.isPlaying) // Check if the sound is already playing
        {
            SoundManger.Instance.emptyMagazineSound.Play();
        }
    }
        if (currentShootingMode == ShootingMode.Auto)
        {
            //Holding the Key
            isShooting = Input.GetKey(KeyCode.Mouse0);    
        } else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst )
        {
            isShooting = Input.GetKeyDown (KeyCode.Mouse0);
        }

        if ((Input.GetKeyDown(KeyCode.R) && magazineBulletsLeft < magazineSize) || (readyToShoot && !isShooting && !isReloading && magazineBulletsLeft <=0 ))
        {
            Reload();
        }

        
        if (readyToShoot && isShooting && magazineBulletsLeft >0)
        {
            burstBulletsLeft = bulletsPerBurst;  
            FireWeapon();
        }


        if  (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{magazineBulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        }

        
    }

    private void FireWeapon()
{
    magazineBulletsLeft --;

    muzzleEffect.GetComponent<ParticleSystem>().Play();
    SoundManger.Instance.shootingSoundAK47.Play();
    readyToShoot = false;

    // Calculate random spread
    Vector3 spread = new Vector3(
        UnityEngine.Random.Range(-spreadIntensity, spreadIntensity),
        UnityEngine.Random.Range(-spreadIntensity, spreadIntensity),
        UnityEngine.Random.Range(-spreadIntensity, spreadIntensity)
    );

    // Create ray direction with spread
    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // Center of the screen
    RaycastHit hit;

    // Apply spread to the ray direction
    Vector3 direction = ray.direction + spread;
    Ray spreadRay = new Ray(ray.origin, direction);

    if (Physics.Raycast(spreadRay, out hit, range))
    {
        Debug.Log("Hit: " + hit.collider.name);

        
        if (!(hit.collider.CompareTag("Zombie")||hit.collider.CompareTag("ZombieHead")))
        {
            CreateBulletImpactEffect(hit);
        }
        // Check if the object hit has the tag "Zombie"
        if (hit.collider.CompareTag("ZombieHead"))
        {
            Debug.Log("zombie head");

            // Get the Zombie script from the parent object of the hit collider
            Zombie zombie = hit.collider.GetComponentInParent<Zombie>();

            // Call the TakeDamage method with double damage if the Zombie script is present
            if (zombie != null)
            {
                int headshotDamage = (int)(weaponDamage * 2); // Apply headshot damage
                zombie.TakeDamage(headshotDamage);
                CreateDamageIndicatorEffect(hit, headshotDamage);
            }

            CreateBloodSplashEffect(hit);
        }
        else if (hit.collider.CompareTag("Zombie"))
        {
            // Get the Zombie script from the hit object
            Zombie zombie = hit.collider.GetComponent<Zombie>();

            // Call the TakeDamage method if the Zombie script is present
            if (zombie != null)
            {
                zombie.TakeDamage((int)weaponDamage);
                CreateDamageIndicatorEffect(hit, (int)weaponDamage);
            }

            CreateBloodSplashEffect(hit);
        }

        // Apply damage to target or other effects here
    }

    if (allowReset)
    {
        Invoke("ResetShot", shootingDelay);
        allowReset = false;
    }

    if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
    {
        burstBulletsLeft--;
        Invoke("FireWeapon", shootingDelay);
    }
}
    private void Reload()
    {
        isReloading = true;
        Invoke ("ReloadCompleted", reloadTime);
        SoundManger.Instance.reloadingSoundAK47.Play();
    }

    private void ReloadCompleted ()
    {
        magazineBulletsLeft = magazineSize;
        isReloading = false;

    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    void CreateBulletImpactEffect(RaycastHit hitInfo)
    {
        // Use the point of impact and normal from RaycastHit
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            hitInfo.point,
            Quaternion.LookRotation(hitInfo.normal)
        );
    }

    void CreateBloodSplashEffect(RaycastHit hitInfo)
    {
        // Use the point of impact and normal from RaycastHit
        GameObject bloodSplash = Instantiate(
            GlobalReferences.Instance.bloodSprayEffectPrefab,
            hitInfo.point,
            Quaternion.LookRotation(hitInfo.normal)
        );
    }

    void CreateDamageIndicatorEffect(RaycastHit hitInfo, int damageAmount)
    {
        // Use the point of impact and normal from RaycastHit
        GameObject indicator = Instantiate(
            GlobalReferences.Instance.damageIndicatorEffectPrefab,
            hitInfo.point,  // Use world position
            Quaternion.identity  // Orientation is not critical for text
        );

        var textMesh = indicator.GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            // Set the text to display the damage amount
            textMesh.text = damageAmount.ToString();

            // Start the fade-out coroutine
        }

        // Ensure it's facing the camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Make the text face the camera
            textMesh.transform.LookAt(mainCamera.transform);
            textMesh.transform.Rotate(0, 180, 0);  // Adjust rotation so the text faces the player correctly
        }
    }

    private void UpdateWeaponStats()
    {
        if (Inventory.Instance.currentWeapon != null)
        {
            WeaponData weaponData = Inventory.Instance.currentWeapon;

            magazineSize = weaponData.magazineSize;
            weaponDamage = weaponData.damage;
            reloadTime = weaponData.reloadTime;

            // Update other weapon properties as needed
        }
    }
}
