using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponStateManager : MonoBehaviour
    {
        private WeaponState[] _weaponStates;
        private WeaponManager _weaponManager;

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
            InitializeWeaponStates();
        }

        private void InitializeWeaponStates()
        {
            // Initialize states for all weapons.
            int weaponCount = _weaponManager.GetWeaponCount();
            _weaponStates = new WeaponState[weaponCount];
            
            for (int i = 0; i < weaponCount; i++)
            {
                WeaponData weaponData = _weaponManager.GetWeaponAtIndex(i);
                if (weaponData != null)
                {
                    _weaponStates[i] = new WeaponState(weaponData);
                }
            }
        }

        public WeaponState GetWeaponState(int index)
        {
            if (index >= 0 && index < _weaponStates.Length)
            {
                return _weaponStates[index];
            }
            return null;
        }

        public void UpdateWeaponState(int index, int currentAmmo, int totalAmmoLeft, bool isReloading)
        {
            if (index >= 0 && index < _weaponStates.Length && _weaponStates[index] != null)
            {
                _weaponStates[index].currentAmmo = currentAmmo;
                _weaponStates[index].totalAmmoLeft = totalAmmoLeft;
                _weaponStates[index].isReloading = isReloading;
            }
        }

        // Method to reset a specific weapon's state to default.
        public void ResetWeaponState(int index)
        {
            if (index >= 0 && index < _weaponStates.Length)
            {
                WeaponData weaponData = _weaponManager.GetWeaponAtIndex(index);
                if (weaponData != null)
                {
                    _weaponStates[index] = new WeaponState(weaponData);
                }
            }
        }

        // Method to reset all weapon states.
        public void ResetAllWeaponStates()
        {
            InitializeWeaponStates();
        }
    }
}