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
        [SerializeField] private float floatingOffset = 0.5f;

        [Header("- Physics Settings")]
        [SerializeField] private float gravity = 20f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float raycastDistance = 10f;

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

        public void OnEnable()
        {

        }
        
        public void Initialize(Vector3 initialVelocity)
        {
            _velocity = initialVelocity;
            _hasLanded = false;
        }

        private void Update()
        {
            PhysicsUpdate();
            CheckForPlayer();
        }
        
        private void PhysicsUpdate()
        {
            if (!_hasLanded)
            {
                // Apply gravity.
                _velocity.y -= gravity * Time.deltaTime;
                
                // Update position.
                Vector3 newPosition = transform.position + _velocity * Time.deltaTime;
                
                // Check for ground using raycast.
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
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
        }

        private void CheckForPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("PlayerCollider"))
                {
                    // Try to find components at the root player object.
                    GameObject playerRoot = col.transform.root.gameObject;
                    WeaponStateManager weaponManager = playerRoot.GetComponent<WeaponStateManager>();
                    
                    if (weaponManager != null)
                    {
                        if (AddAmmoToCurrentWeapon(weaponManager))
                        {
                            PlayPickupEffects();
                            Destroy(gameObject);
                        }
                    }
                    
                    break;
                }
            }
        }
        
        private bool AddAmmoToCurrentWeapon(WeaponStateManager weaponManager)
        {
            // Get the components directly from the same GameObject.
            WeaponManager weaponManagerComp = weaponManager.GetComponent<WeaponManager>();
            WeaponHandler weaponHandler = weaponManager.GetComponent<WeaponHandler>();
            
            if (weaponManagerComp == null || weaponHandler == null)
            {
                return false;
            }
            
            int currentWeaponIndex = weaponManagerComp.GetCurrentWeaponIndex();
            WeaponState currentState = weaponManager.GetWeaponState(currentWeaponIndex);
            WeaponData weaponData = weaponHandler.GetCurrentWeaponData();
            
            if (currentState == null || weaponData == null)
            {
                return false;
            }
            
            // Calculate max possible ammo.
            int maxPossibleAmmo = weaponData.maxAmmo;
            int currentTotal = currentState.currentAmmo + currentState.totalAmmoLeft;
            int ammoSpace = maxPossibleAmmo - currentTotal;
            
            // Only add ammo if there's space.
            if (ammoSpace <= 0)
            {
                return false;
            }
            
            // Add ammo to reserve.
            int ammoToAdd = Mathf.Min(ammoAmount, ammoSpace);
            currentState.totalAmmoLeft += ammoToAdd;
            
            // Update the weapon state.
            weaponManager.UpdateWeaponState(
                currentWeaponIndex,
                currentState.currentAmmo,
                currentState.totalAmmoLeft,
                currentState.isReloading
            );
            
            return true;
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
        
        public void LogWeaponState(WeaponStateManager manager)
        {
            if (manager == null) return;
    
            WeaponManager weaponManager = manager.GetComponent<WeaponManager>();
            if (weaponManager == null) return;
    
            int index = weaponManager.GetCurrentWeaponIndex();
            WeaponState state = manager.GetWeaponState(index);
    
            if (state != null)
            {
                string weaponName = "Unknown";
                WeaponHandler handler = manager.GetComponent<WeaponHandler>();
                if (handler != null && handler.GetCurrentWeaponData() != null)
                {
                    weaponName = handler.GetCurrentWeaponData().weaponName;
                }
        
                Debug.Log($"======== WEAPON STATE ========");
                Debug.Log($"Weapon: {weaponName} (Index: {index})");
                Debug.Log($"Magazine: {state.currentAmmo}");
                Debug.Log($"Reserve: {state.totalAmmoLeft}");
                Debug.Log($"Is Reloading: {state.isReloading}");
                Debug.Log($"==============================");
            }
        }
    }
}