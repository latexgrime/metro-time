using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private List<WeaponData> availableWeapons;

        private Weapon _currentWeapon;
        private int _currentWeaponIndex;

        private void Start()
        {
            if (availableWeapons.Count > 0) EquipWeapon(0);
        }

        public void EquipWeapon(int index)
        {
            if (index < 0 || index >= availableWeapons.Count) return;

            if (_currentWeapon != null) Destroy(_currentWeapon.gameObject);

            _currentWeaponIndex = index;

            // Null checks.
            if (availableWeapons[index] == null || availableWeapons[index].weaponPrefab == null)
            {
                Debug.LogError("Weapon Data or Prefab is null!");
                return;
            }

            var weaponInstance = Instantiate(availableWeapons[index].weaponPrefab, weaponHolder);
            if (weaponInstance == null)
            {
                Debug.LogError($"Failed to instantiate weapon prefab for {availableWeapons[index].weaponName}");
                return;
            }

            // Null check for Weapon component.
            _currentWeapon = weaponInstance.GetComponent<Weapon>();
            if (_currentWeapon == null)
            {
                Debug.LogError("Weapon component not found on instantiated prefab!");
                Destroy(weaponInstance);
                return;
            }

            // Notify WeaponHandler of the change.
            var handler = GetComponent<WeaponHandler>();
            if (handler != null) handler.OnWeaponEquipped(_currentWeapon);
        }

        public Weapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }
        
        public int GetCurrentWeaponIndex()
        {
            return _currentWeaponIndex;
        }

        public int GetWeaponCount()
        {
            return availableWeapons.Count;
        }
        
        public WeaponData GetWeaponAtIndex(int index)
        {
            if (index >= 0 && index < availableWeapons.Count)
                return availableWeapons[index];
            return null;
        }
    }
}