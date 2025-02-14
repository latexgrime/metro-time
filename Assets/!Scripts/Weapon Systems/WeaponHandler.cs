using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Weapon_Systems
{
    public class WeaponHandler : MonoBehaviour
    {
        private InputManager _inputManager;
        private WeaponManager _weaponManager;
        private UnityEngine.Camera _mainCamera;
        private AudioSource _audioSource;

        private Weapon _currentWeapon;
        private WeaponData _currentWeaponData;

        private float _nextTimeToFire;
        private bool _isReloading;
        private int _currentAmmo;

        [Header("State")] public bool canShoot = true;

        [Header("Debug Settings")] [SerializeField]
        private bool showDebugHits;

        [SerializeField] private float debugLineDuration = 1f;
        [SerializeField] private Color debugLineColor = Color.red;
        [SerializeField] private bool showHitInfo = true;

        private void Start()
        {
            _inputManager = GetComponent<InputManager>();
            _weaponManager = GetComponent<WeaponManager>();
            _mainCamera = UnityEngine.Camera.main;
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnWeaponEquipped(Weapon weapon)
        {
            _currentWeapon = weapon;
            _currentWeaponData = weapon.GetWeaponData();
            _currentAmmo = _currentWeaponData.magazineSize;
            _isReloading = false;
            _nextTimeToFire = 0f;
        }

        private void Update()
        {
            if (_currentWeapon == null || _isReloading || !canShoot) return;
            
            if (_inputManager.shootInput)
            {
                if (_currentWeaponData.isAutomatic)
                {
                    if (Time.time >= _nextTimeToFire) Shoot();
                }
                else if (Time.time >= _nextTimeToFire)
                {
                    Shoot();
                }
            }
            
            if (_inputManager.reloadInput && !_isReloading)
                StartReload();
        }

        private void Shoot()
        {
            if (_currentAmmo <= 0)
            {
                StartReload();
                return;
            }

            _nextTimeToFire = Time.time + 1f / _currentWeaponData.fireRate;
            _currentAmmo--;

            // Play effects.
            _currentWeapon.PlayMuzzleFlash();
            if (_currentWeaponData.shootSound != null)
            {
                _audioSource.PlayOneShot(_currentWeaponData.shootSound);
            }

            // Handle shooting logic.
            if (_currentWeaponData.usePhysicalBullets)
            {
                ShootPhysicalBullet();
            }
            else
            {
                ShootRaycast();
            }

            // Apply recoil.
            WeaponRecoil recoil = GetComponent<WeaponRecoil>();
            if (recoil != null)
            {
                recoil.ApplyRecoil(_currentWeaponData);
            }
        }

        private void ShootPhysicalBullet()
        {
            // Get the shoot point position.
            Transform shootPoint = _currentWeapon.GetShootPoint();

            // Create a ray from the center of the screen.
            Ray centerRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            // Calculate direction from shoot point to the point the ray hits.
            Vector3 direction;
            RaycastHit hit;
            if (Physics.Raycast(centerRay, out hit, _currentWeaponData.range))
            {
                // Direction is from shoot point to the hit point.
                direction = (hit.point - shootPoint.position).normalized;
            }
            else
            {
                // If no hit, use the ray direction.
                direction = centerRay.direction;
            }

            // Apply spread.
            if (_currentWeaponData.spread > 0)
            {
                direction += Random.insideUnitSphere * _currentWeaponData.spread;
                direction.Normalize();
            }

            // Create bullet.
            GameObject bullet = Instantiate(_currentWeaponData.bulletPrefab, 
                shootPoint.position, Quaternion.LookRotation(direction));

            // Set up bullet properties.
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.damage = _currentWeaponData.damage;
                bulletComponent.speed = _currentWeaponData.bulletSpeed;
                bulletComponent.lifetime = _currentWeaponData.bulletLifetime;
                bulletComponent.impactEffect = _currentWeaponData.impactEffectPrefab;
            }

            // Add force to bullet.
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = direction * _currentWeaponData.bulletSpeed;
            }
        }

        private void ShootRaycast()
        {
            // Implementation of raycast in case the physical bullet brings the performance down too much.
            Ray centerRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    
            // Use the ray for the raycast instead of camera's forward direction
            if (Physics.Raycast(centerRay, out RaycastHit hit, _currentWeaponData.range))
            {
                HandleHit(hit);
            }
        }
        private Vector3 CalculateShootDirection()
        {
            // Create a ray from the center of the screen.
            Ray centerRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    
            // Calculate spread.
            Vector3 direction = centerRay.direction;
            if (_currentWeaponData.spread > 0)
            {
                direction += Random.insideUnitSphere * _currentWeaponData.spread;
                direction.Normalize();
            }

            return direction;
        }

        private void HandleHit(RaycastHit hit)
        {
            if (showDebugHits)
            {
                // Draw debug line from camera to hit point.
                Debug.DrawLine(_mainCamera.transform.position, hit.point, debugLineColor, debugLineDuration);

                // Draw hit point sphere.
                Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.yellow, debugLineDuration);

                if (showHitInfo)
                    Debug.Log($"Hit object: {hit.collider.gameObject.name} " +
                              $"at position: {hit.point} " +
                              $"with surface normal: {hit.normal} " +
                              $"Material/Tag: {hit.collider.tag}");
            }

            // Handle impact effects.
            if (_currentWeaponData.impactEffectPrefab != null)
                Instantiate(_currentWeaponData.impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

            // Handle damage.
            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(_currentWeaponData.damage);
        }

        private void StartReload()
        {
            if (_currentAmmo == _currentWeaponData.magazineSize) return;

            _isReloading = true;
    
            // Play reload sound.
            if (_currentWeaponData.reloadSound != null) 
                _audioSource.PlayOneShot(_currentWeaponData.reloadSound);
    
            // Get the Animator component.
            Animator animator = _currentWeapon.GetComponent<Animator>();
            if (animator != null)
            {
                // Trigger reload animation.
                animator.SetTrigger("Reload");
            }

            Invoke(nameof(FinishReload), _currentWeaponData.reloadTime);
        }

        private void FinishReload()
        {
            _currentAmmo = _currentWeaponData.magazineSize;
            _isReloading = false;
    
            // Reset reload animation.
            Animator animator = _currentWeapon.GetComponent<Animator>();
            if (animator != null)
            {
                animator.ResetTrigger("Reload");
            }
        }
        
        public Weapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }
    }
}