using _Scripts.Weapon_Systems.Weapons_Logic;
using UnityEngine;

namespace _Scripts.AmmoDrop
{
    public class AmmoPickup : MonoBehaviour
    {
        [Header("- Pickup Settings")]
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float pickupRadius = 2f;
        [SerializeField] private float floatingOffset = .5f;

        [Header("- Physics Settings")]
        [SerializeField] private float gravity = 20f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float _raycastDistance = 10f;

        [Header("- Ammo Settings")]
        [SerializeField] private int ammoAmount = 30;

        [Header("- Effects")]
        [SerializeField] private GameObject pickupEffect;
        [SerializeField] private AudioClip pickupSound;

        private Vector3 _velocity;
        private bool _hasLanded;
        private Vector3 _startPosition;
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void Initialize(Vector3 initialVelocity)
        {
            _velocity = initialVelocity;
            _hasLanded = false;
        }

        private void Update()
        {
            if (!_hasLanded)
            {
                // Apply gravity.
                _velocity.y -= gravity * Time.deltaTime;

                // Update position.
                Vector3 newPosition = transform.position + _velocity * Time.deltaTime;
                
                // Check for ground using raycast.
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, _raycastDistance, groundLayer))
                {
                    // If we're about to go below ground level, land the ammo with offset.
                    if (newPosition.y <= hit.point.y + floatingOffset)
                    {
                        newPosition.y = hit.point.y + floatingOffset;
                        _hasLanded = true;
                        _startPosition = newPosition;
                        _velocity = Vector3.zero;
                    }
                }
                
                transform.position = newPosition;
            }
            else
            {
                // Rotate the pickup.
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

                // Bob up and down around the floating offset.
                float newY = _startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }

            // Check for nearby player - now checks the parent of colliders for WeaponStateManager.
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
            foreach (Collider col in colliders)
            {
                // Check the collider's object and its parent for the WeaponStateManager.
                WeaponStateManager weaponManager = col.GetComponent<WeaponStateManager>();
                if (weaponManager == null && col.transform.parent != null)
                {
                    weaponManager = col.transform.parent.GetComponent<WeaponStateManager>();
                }

                if (weaponManager != null)
                {
                    AddAmmoToWeapons(weaponManager);
                    PlayPickupEffects();
                    Destroy(gameObject);
                    break;
                }
            }
        }

        private void AddAmmoToWeapons(WeaponStateManager weaponManager)
        {
            var weaponHandler = weaponManager.GetComponent<WeaponHandler>();
            if (weaponHandler == null) return;

            var currentWeaponIndex = weaponManager.GetComponent<WeaponManager>().GetCurrentWeaponIndex();
            var currentState = weaponManager.GetWeaponState(currentWeaponIndex);

            if (currentState != null)
            {
                var weaponData = weaponHandler.GetCurrentWeaponData();
                if (weaponData != null)
                {
                    currentState.totalAmmoLeft = Mathf.Min(
                        currentState.totalAmmoLeft + ammoAmount,
                        weaponData.maxAmmo - currentState.currentAmmo
                    );

                    weaponManager.UpdateWeaponState(
                        currentWeaponIndex,
                        currentState.currentAmmo,
                        currentState.totalAmmoLeft,
                        currentState.isReloading
                    );
                }
            }
        }

        public void SetAmmoAmount(int amount)
        {
            ammoAmount = amount;
        }

        private void PlayPickupEffects()
        {
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
        } 
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }
}