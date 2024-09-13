using System;
using TMPro;
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

    public float reloadtime;
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

        
        if (!hit.collider.CompareTag("Zombie"))
        {
            CreateBulletImpactEffect(hit);
        }
        // Check if the object hit has the tag "Zombie"
        if (hit.collider.CompareTag("Zombie"))
        {
            // Get the Zombie script from the hit object
            Zombie zombie = hit.collider.GetComponent<Zombie>();

            // Call the TakeDamage method if the Zombie script is present
            if (zombie != null)
            {
                zombie.TakeDamage((int)weaponDamage); // Assuming weaponDamage is a float, cast it to int
            }
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
        Invoke ("ReloadCompleted", reloadtime);
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
}
