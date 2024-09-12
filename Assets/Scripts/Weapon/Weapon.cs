using System;
    using UnityEngine;

public class Weapon : MonoBehaviour
{

    //Shooting
    public float range = 1000f;
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft; 

    //Spread
    public float spreadIntensity;

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
    }
   

    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            //Holding the Key
            isShooting = Input.GetKey(KeyCode.Mouse0);    
        } else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst )
        {
            isShooting = Input.GetKeyDown (KeyCode.Mouse0);
        }

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;  
            FireWeapon();
        }
        
    }

   private void FireWeapon()
{
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

        CreateBulletImpactEffect(hit);
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
