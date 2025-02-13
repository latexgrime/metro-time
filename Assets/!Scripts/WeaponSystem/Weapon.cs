using System;
using System.Collections;
using UnityEngine;

namespace Dyson.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public Camera playerCamera;
        #region Shooting variables
        [Header("Shooting Settings")]
        public bool isShooting, readyToShoot;
        private bool allowReset = true;
        public float shootingDelay = 2f;
        #endregion
        
        #region Bullet variables
        [Header("Bullets Settings")]
        public GameObject bulletPrefab;

        private GameObject instantiated;

        public Transform bulletSpawn;

        public float bulletVelocity = 30f;

        public float bulletPrefabLifeTime = 3f;
        
        //Burst
        public int bulletsPerBurst = 3;
        public int burstBulletsLeft;
        
        //Spread
        public float spreadIntensity;
        #endregion


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
        
        // Update is called once per frame
        void FixedUpdate()
        {
            if (currentShootingMode == ShootingMode.Auto)
            {
                //Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else  if (currentShootingMode == ShootingMode.Single ||
                      currentShootingMode == ShootingMode.Burst)
            {
                //Clicking only once 
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (readyToShoot && isShooting)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
           // animator.SetBool("fire", false);
        }

        private void FireWeapon()
        {
            readyToShoot = false;
            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletPrefab.transform.rotation);

            bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
            bullet.transform.forward = shootingDirection;
            
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

            if (allowReset)
            {
                Invoke("ResetShot", shootingDelay);
                allowReset = false;
            }

            if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
            {
                burstBulletsLeft--;
                Invoke("FireWeapon", burstBulletsLeft);
            }
        }

        private void ResetShot()
        {
            readyToShoot = true;
            allowReset = true;
        }

        private Vector3 CalculateDirectionAndSpread()
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit))
            {
                //Hitting something
                targetPoint = hit.point;
                //animator.SetBool("fire", true);
            }
            else
            {
                //Shooting at the air
                targetPoint = ray.GetPoint(100);
            }

            Vector3 direction = targetPoint - bulletSpawn.position;

            float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
            float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
            
            //Returning the shoot direction and spread
            return direction + new Vector3(x, y, 0);
        }

        private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(bullet);
        }
    }
}
