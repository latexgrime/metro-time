using _Scripts.Enemy.Interfaces;
using _Scripts.Player;
using _Scripts.Player.Movement;
using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponHandler : MonoBehaviour
    {
        [Header("- Dependencies")]
        private PlayerMovement _playerMovement;
        private InputManager _inputManager;
        private WeaponManager _weaponManager;
        private UnityEngine.Camera _mainCamera;
        private AudioSource _audioSource;
        private WeaponStateManager _weaponStateManager;

        [Header("- Weapon State")]
        private Weapon _currentWeapon;
        private WeaponData _currentWeaponData;
        private float _nextTimeToFire;
        private bool _isReloading;
        private int _currentAmmo;
        private int _totalAmmoLeft;
        private float _lastScrollValue;
        private bool _hasShot;

        [Header("- Shooting Settings")]
        public bool canShoot = true;

        [Header("- Debug Settings")]
        [SerializeField] private bool showDebugHits;
        [SerializeField] private float debugLineDuration = 1f;
        [SerializeField] private Color debugLineColor = Color.red;
        [SerializeField] private bool showHitInfo = true;

        private void Start()
        {
            InitializeDependencies();
        }

        private void InitializeDependencies()
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

            // Retrieve saved weapon state.
            RestoreWeaponState();

            _nextTimeToFire = 0f;
        }

        private void RestoreWeaponState()
        {
            WeaponState state = _weaponStateManager.GetWeaponState(_weaponManager.GetCurrentWeaponIndex());
            
            if (state != null)
            {
                _currentAmmo = state.currentAmmo;
                _totalAmmoLeft = state.totalAmmoLeft;
                _isReloading = state.isReloading;
            }
            else
            {
                // Initialize with default values if no state exists.
                _currentAmmo = _currentWeaponData.magazineSize;
                _totalAmmoLeft = _currentWeaponData.maxAmmo - _currentWeaponData.magazineSize;
                _isReloading = false;
            }
        }

        private void Update()
        {
            if (_currentWeapon == null || _isReloading || !canShoot) return;

            HandleShooting();
            HandleReloading();
            HandleWeaponSwitch();
        }

        private void HandleShooting()
        {
            // Prevent shooting while sprinting.
            if (_playerMovement.CurrentState == PlayerState.Sprinting) return;

            if (_inputManager.shootInput)
            {
                // Force walk state if shooting while sprinting.
                if (_playerMovement.CurrentState == PlayerState.Sprinting)
                {
                    _playerMovement.ForceWalkState();
                }

                // Handle weapon fire modes.
                if ((_currentWeaponData.isAutomatic && Time.time >= _nextTimeToFire) ||
                    (!_currentWeaponData.isAutomatic && Time.time >= _nextTimeToFire && !_hasShot))
                {
                    Shoot();
                }
            }
            else
            {
                _hasShot = false;
            }
        }

        private void HandleReloading()
        {
            if (_inputManager.reloadInput && !_isReloading)
            {
                StartReload();
            }
        }

        private void HandleWeaponSwitch()
        {
            if (_inputManager.weaponScrollInput != 0 && _lastScrollValue == 0)
            {
                int currentIndex = _weaponManager.GetCurrentWeaponIndex();
                int numberOfWeapons = _weaponManager.GetWeaponCount();

                int newIndex = _inputManager.weaponScrollInput > 0
                    ? (currentIndex + 1) % numberOfWeapons
                    : (currentIndex - 1 + numberOfWeapons) % numberOfWeapons;

                _weaponManager.EquipWeapon(newIndex);
            }

            _lastScrollValue = _inputManager.weaponScrollInput;
        }

        private void Shoot()
        {
            // Ensure we can shoot.
            if (_currentAmmo <= 0)
            {
                SaveCurrentWeaponState();
                StartReload();
                return;
            }

            // Update shooting parameters.
            _nextTimeToFire = Time.time + 1f / _currentWeaponData.fireRate;
            _currentAmmo--;

            // Play shooting effects.
            PlayShootingEffects();

            // Handle bullet/raycast logic.
            if (_currentWeaponData.usePhysicalBullets)
            {
                ShootPhysicalBullet();
            }
            else
            {
                ShootRaycast();
            }

            // Apply weapon recoil.
            ApplyRecoil();

            // Save weapon state.
            SaveCurrentWeaponState();
        }

        private void PlayShootingEffects()
        {
            _currentWeapon.PlayMuzzleFlash();
            
            if (_currentWeaponData.shootSound != null)
            {
                _audioSource.PlayOneShot(_currentWeaponData.shootSound);
            }
        }

        private void ApplyRecoil()
        {
            WeaponRecoil recoil = GetComponent<WeaponRecoil>();
            if (recoil != null)
            {
                recoil.ApplyRecoil(_currentWeaponData);
            }
        }

        private void ShootPhysicalBullet()
        {
            Transform shootPoint = _currentWeapon.GetShootPoint();
            Ray centerRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            
            Vector3 direction = CalculateShootDirection(centerRay);
            
            CreatePhysicalBullet(shootPoint, direction);
        }

        private Vector3 CalculateShootDirection(Ray centerRay)
        {
            RaycastHit hit;
            Vector3 direction = centerRay.direction;

            if (Physics.Raycast(centerRay, out hit, _currentWeaponData.range))
            {
                direction = (hit.point - _currentWeapon.GetShootPoint().position).normalized;
            }

            // Apply spread.
            if (_currentWeaponData.spread > 0)
            {
                direction += Random.insideUnitSphere * _currentWeaponData.spread;
                direction.Normalize();
            }

            return direction;
        }

        private void CreatePhysicalBullet(Transform shootPoint, Vector3 direction)
        {
            GameObject bullet = Instantiate(
                _currentWeaponData.bulletPrefab, 
                shootPoint.position, 
                Quaternion.LookRotation(direction)
            );

            ConfigureBulletProperties(bullet, direction);
        }

        private void ConfigureBulletProperties(GameObject bullet, Vector3 direction)
        {
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.damage = _currentWeaponData.damage;
                bulletComponent.speed = _currentWeaponData.bulletSpeed;
                bulletComponent.lifetime = _currentWeaponData.bulletLifetime;
                bulletComponent.impactEffect = _currentWeaponData.impactEffectPrefab;
            }

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = direction * _currentWeaponData.bulletSpeed;
            }
        }

        private void ShootRaycast()
        {
            Ray centerRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            
            if (Physics.Raycast(centerRay, out RaycastHit hit, _currentWeaponData.range))
            {
                HandleRaycastHit(hit);
            }
        }

        private void HandleRaycastHit(RaycastHit hit)
        {
            // Debug hit visualization.
            VisualizeDebugHit(hit);

            // Handle impact effects.
            if (_currentWeaponData.impactEffectPrefab != null)
            {
                Instantiate(_currentWeaponData.impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Handle damage.
            var damageable = hit.collider.GetComponent<IShieldable>();
            damageable?.TakeShieldDamage(_currentWeaponData.damage);
        }

        private void VisualizeDebugHit(RaycastHit hit)
        {
            if (!showDebugHits) return;

            Debug.DrawLine(_mainCamera.transform.position, hit.point, debugLineColor, debugLineDuration);
            Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.yellow, debugLineDuration);

            if (showHitInfo)
            {
                Debug.Log($"Hit object: {hit.collider.gameObject.name} " +
                          $"at position: {hit.point} " +
                          $"with surface normal: {hit.normal} " +
                          $"Material/Tag: {hit.collider.tag}");
            }
        }

        private void StartReload()
        {
            // Only reload if magazine isn't full and we have ammo.
            if (_currentAmmo == _currentWeaponData.magazineSize || _totalAmmoLeft <= 0) return;

            _isReloading = true;

            // Play reload sound.
            if (_currentWeaponData.reloadSound != null)
            {
                _audioSource.PlayOneShot(_currentWeaponData.reloadSound);
            }

            // Trigger reload animation.
            Animator animator = _currentWeapon.GetAnimator();
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
            Animator animator = _currentWeapon.GetAnimator();
            if (animator != null)
            {
                animator.ResetTrigger("Reload");
            }

            SaveCurrentWeaponState();
        }

        private void SaveCurrentWeaponState()
        {
            _weaponStateManager?.UpdateWeaponState(
                _weaponManager.GetCurrentWeaponIndex(),
                _currentAmmo,
                _totalAmmoLeft,
                _isReloading
            );
        }

        // Getters for UI and external access.
        public Weapon GetCurrentWeapon() => _currentWeapon;
        public int GetCurrentAmmo() => _currentAmmo;
        public int GetTotalAmmoLeft() => _totalAmmoLeft;
        public int GetMagazineSize() => _currentWeaponData?.magazineSize ?? 0;
        public WeaponData GetCurrentWeaponData() => _currentWeaponData;
    }
}