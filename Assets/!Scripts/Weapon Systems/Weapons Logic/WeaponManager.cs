using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("- Weapon Configuration")]
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private List<WeaponData> availableWeapons;

        private Weapon _currentWeapon;
        private int _currentWeaponIndex = -1;

        private void Start()
        {
            // Equip first weapon if available.
            if (availableWeapons.Count > 0)
            {
                EquipWeapon(0);
            }
        }

        public void EquipWeapon(int index)
        {
            // Validate weapon index.
            if (index < 0 || index >= availableWeapons.Count) return;

            // Destroy current weapon if exists.
            if (_currentWeapon != null)
            {
                Destroy(_currentWeapon.gameObject);
            }

            // Validate weapon data.
            WeaponData weaponData = availableWeapons[index];
            if (weaponData == null || weaponData.weaponPrefab == null)
            {
                Debug.LogError($"Invalid weapon data at index {index}.");
                return;
            }

            // Instantiate new weapon.
            GameObject weaponInstance = InstantiateWeapon(weaponData);
            if (weaponInstance == null) return;

            // Update current weapon state.
            _currentWeaponIndex = index;
            _currentWeapon = weaponInstance.GetComponent<Weapon>();

            // Notify weapon handler.
            NotifyWeaponHandler();
        }

        private GameObject InstantiateWeapon(WeaponData weaponData)
        {
            GameObject weaponInstance = Instantiate(weaponData.weaponPrefab, weaponHolder);
            
            if (weaponInstance == null)
            {
                Debug.LogError($"Failed to instantiate weapon: {weaponData.weaponName}.");
                return null;
            }

            Weapon weaponComponent = weaponInstance.GetComponent<Weapon>();
            if (weaponComponent == null)
            {
                Debug.LogError($"Weapon component not found on {weaponData.weaponName} prefab.");
                Destroy(weaponInstance);
                return null;
            }

            return weaponInstance;
        }

        private void NotifyWeaponHandler()
        {
            WeaponHandler handler = GetComponent<WeaponHandler>();
            handler?.OnWeaponEquipped(_currentWeapon);
        }

        // Getter methods for external access.
        public Weapon GetCurrentWeapon() => _currentWeapon;
        
        public int GetCurrentWeaponIndex() => _currentWeaponIndex;

        public int GetWeaponCount() => availableWeapons.Count;
        
        public WeaponData GetWeaponAtIndex(int index)
        {
            if (index >= 0 && index < availableWeapons.Count)
                return availableWeapons[index];
            return null;
        }
    }
}