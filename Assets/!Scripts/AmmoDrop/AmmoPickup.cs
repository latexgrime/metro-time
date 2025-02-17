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
        
        [Header("- Ammo Settings")]
        [SerializeField] private int ammoAmount = 30;
        
        [Header("- Effects")]
        [SerializeField] private GameObject pickupEffect;
        [SerializeField] private AudioClip pickupSound;
        
        private Vector3 _startPosition;
        private AudioSource _audioSource;
        
        private void Start()
        {
            _startPosition = transform.position;
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        private void Update()
        {
            // Rotate the pickup.
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            // Bob up and down.
            float newY = _startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            
            // Check for nearby player.
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    WeaponStateManager weaponManager = col.GetComponent<WeaponStateManager>();
                    if (weaponManager != null)
                    {
                        AddAmmoToWeapons(weaponManager);
                        PlayPickupEffects();
                        Destroy(gameObject);
                        break;
                    }
                }
            }
        }
        
        private void AddAmmoToWeapons(WeaponStateManager weaponManager)
        {
            // Get current weapon index and state.
            WeaponHandler weaponHandler = weaponManager.GetComponent<WeaponHandler>();
            if (weaponHandler == null) return;
            
            int currentWeaponIndex = weaponManager.GetComponent<WeaponManager>().GetCurrentWeaponIndex();
            WeaponState currentState = weaponManager.GetWeaponState(currentWeaponIndex);
            
            if (currentState != null)
            {
                WeaponData weaponData = weaponHandler.GetCurrentWeaponData();
                if (weaponData != null)
                {
                    // Add ammo up to the max.
                    currentState.totalAmmoLeft = Mathf.Min(
                        currentState.totalAmmoLeft + ammoAmount,
                        weaponData.maxAmmo - currentState.currentAmmo
                    );
                    
                    // Update the weapon state.
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
            // Draw pickup radius in editor.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }
}
