using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponHandler : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private InputManager _inputManager;
        private WeaponManager _weaponManager;
        private UnityEngine.Camera _mainCamera;
        private AudioSource _audioSource;
        private PlayerControls _playerControls;
        private WeaponStateManager _weaponStateManager;

        private Weapon _currentWeapon;
        private WeaponData _currentWeaponData;

        private float _nextTimeToFire;
        private bool _isReloading;
        private int _currentAmmo;
        private int _totalAmmoLeft;
        private float _lastScrollValue = 0f;
        private bool _hasShot;
        
        

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
            _playerMovement = GetComponent<PlayerMovement>();
            _weaponStateManager = GetComponent<WeaponStateManager>();
        }

        public void OnWeaponEquipped(Weapon weapon)
        {
            _currentWeapon = weapon;
            _currentWeaponData = weapon.GetWeaponData();
    
            // Get the saved state for this weapon.
            WeaponState state = _weaponStateManager.GetWeaponState(_weaponManager.GetCurrentWeaponIndex());
            if (state != null)
            {
                _currentAmmo = state.currentAmmo;
                _totalAmmoLeft = state.totalAmmoLeft;
                _isReloading = state.isReloading;
            }
            else
            {
                // To avoid errors: fallback to default values if no state exists.
                _currentAmmo = _currentWeaponData.magazineSize;
                _totalAmmoLeft = _currentWeaponData.maxAmmo - _currentWeaponData.magazineSize;
                _isReloading = false;
            }
            _nextTimeToFire = 0f;
        }
        
        private void Update()
        {
            if (_currentWeapon == null || _isReloading || !canShoot) return;
        
            // Check if player is sprinting.
            if (_playerMovement.CurrentState == PlayerState.Sprinting)
            {
                // Don't allow shooting while sprinting.
                return;
            }
        
            if (_inputManager.shootInput)
            {
                // If player starts shooting while sprinting, force them to walk.
                if (_playerMovement.CurrentState == PlayerState.Sprinting)
                {
                    _playerMovement.ForceWalkState();
                }

                // Handle weapon shoot mode [Automatic or not]. 
                if (_currentWeaponData.isAutomatic && Time.time >= _nextTimeToFire)
                {
                    Shoot();
                }
                else if (!_currentWeaponData.isAutomatic && Time.time >= _nextTimeToFire && !_hasShot)
                {
                    Shoot();
                    _hasShot = true;
                }
            }
            else
            {
                _hasShot = false;
            }
        
            if (_inputManager.reloadInput && !_isReloading)
                StartReload();

            // Handle weapon switching - only trigger on scroll value change.
            if (_inputManager.weaponScrollInput != 0 && _lastScrollValue == 0)
            {
                int currentIndex = _weaponManager.GetCurrentWeaponIndex();
                int numberOfWeapons = _weaponManager.GetWeaponCount();
            
                if (_inputManager.weaponScrollInput > 0)
                {
                    int newIndex = (currentIndex + 1) % numberOfWeapons;
                    _weaponManager.EquipWeapon(newIndex);
                }
                else
                {
                    int newIndex = (currentIndex - 1 + numberOfWeapons) % numberOfWeapons;
                    _weaponManager.EquipWeapon(newIndex);
                }
            }
        
            _lastScrollValue = _inputManager.weaponScrollInput;
        }

        private void Shoot()
        {
            if (_currentAmmo <= 0)
            {
                SaveCurrentWeaponState();
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
            // Only reload if we have ammo left and magazine isn't full.
            if (_currentAmmo == _currentWeaponData.magazineSize || _totalAmmoLeft <= 0) return;

            _isReloading = true;
    
            // Play reload sound.
            if (_currentWeaponData.reloadSound != null) 
                _audioSource.PlayOneShot(_currentWeaponData.reloadSound);
    
            // Get the Animator component.
            Animator animator = _currentWeapon.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Reload");
            }

            Invoke(nameof(FinishReload), _currentWeaponData.reloadTime);
        }

        private void FinishReload()
        {
            int ammoNeeded = _currentWeaponData.magazineSize - _currentAmmo;
            int ammoToAdd = Mathf.Min(ammoNeeded, _totalAmmoLeft);
        
            _currentAmmo += ammoToAdd;
            _totalAmmoLeft -= ammoToAdd;
        
            _isReloading = false;
    
            // Reset reload animation.
            Animator animator = _currentWeapon.GetComponent<Animator>();
            if (animator != null)
            {
                animator.ResetTrigger("Reload");
            }
            
            SaveCurrentWeaponState();
        }
        
        private void SaveCurrentWeaponState()
        {
            if (_weaponStateManager != null)
            {
                _weaponStateManager.UpdateWeaponState(
                    _weaponManager.GetCurrentWeaponIndex(),
                    _currentAmmo,
                    _totalAmmoLeft,
                    _isReloading
                );
            }
        }
        
        
        // Stuff for UI access [to be implemented].
        public Weapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }
        
        public int GetCurrentAmmo()
        {
            return _currentAmmo;
        }

        public int GetTotalAmmoLeft()
        {
            return _totalAmmoLeft;
        }

        public int GetMagazineSize()
        {
            return _currentWeaponData?.magazineSize ?? 0;
        }

        public WeaponData GetCurrentWeaponData()
        {
            return _currentWeaponData;
        }
    }
}