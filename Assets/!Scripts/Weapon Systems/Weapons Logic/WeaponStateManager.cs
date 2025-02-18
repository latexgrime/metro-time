using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponStateManager : MonoBehaviour
    {
        [Header("- Weapon State Management")]
        private WeaponState[] _weaponStates;
        private WeaponManager _weaponManager;

        private void Awake()
        {
            InitializeWeaponManager();
            InitializeWeaponStates();
        }

        private void InitializeWeaponManager()
        {
            _weaponManager = GetComponent<WeaponManager>();
            if (_weaponManager == null)
            {
                Debug.LogError("WeaponManager component not found.");
            }
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
                else
                {
                    Debug.LogWarning($"Weapon data is null at index {i}.");
                }
            }
        }

        /// <summary>
        /// Logs the current state of all weapons for debugging purposes.
        /// </summary>
        public void DebugLogAllWeaponStates()
        {
            if (_weaponStates == null || _weaponStates.Length == 0)
            {
                Debug.LogWarning("No weapon states initialized.");
                return;
            }
    
            Debug.Log("======= WEAPON STATES =======");
            for (int i = 0; i < _weaponStates.Length; i++)
            {
                WeaponState state = _weaponStates[i];
                if (state == null)
                {
                    Debug.Log($"Weapon {i}: NULL STATE");
                    continue;
                }
        
                WeaponData data = _weaponManager.GetWeaponAtIndex(i);
                string name = data != null ? data.weaponName : "Unknown";
        
                Debug.Log($"Weapon {i}: {name}");
                Debug.Log($"  - Current Ammo: {state.currentAmmo}");
                Debug.Log($"  - Reserve Ammo: {state.totalAmmoLeft}");
                Debug.Log($"  - Is Reloading: {state.isReloading}");
            }
            Debug.Log("============================");
        }
        
        /// <summary>
        /// Retrieves the weapon state for a specific weapon index.
        /// </summary>
        public WeaponState GetWeaponState(int index)
        {
            if (index >= 0 && index < _weaponStates.Length)
            {
                return _weaponStates[index];
            }
            return null;
        }

        /// <summary>
        /// Updates the state of a specific weapon.
        /// </summary>
        public void UpdateWeaponState(int index, int currentAmmo, int totalAmmoLeft, bool isReloading)
        {
            if (index >= 0 && index < _weaponStates.Length && _weaponStates[index] != null)
            {
                _weaponStates[index].currentAmmo = currentAmmo;
                _weaponStates[index].totalAmmoLeft = totalAmmoLeft;
                _weaponStates[index].isReloading = isReloading;
            }
        }

        /// <summary>
        /// Resets the state of a specific weapon to its default configuration.
        /// </summary>
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

        /// <summary>
        /// Resets all weapon states to their default configurations.
        /// </summary>
        public void ResetAllWeaponStates()
        {
            InitializeWeaponStates();
        }
    }
}