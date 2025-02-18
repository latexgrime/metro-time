using System;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    /// <summary>
    /// Represents the current state of a weapon, including ammo and reload status.
    /// </summary>
    [Serializable]
    public class WeaponState
    {
        /// <summary>
        /// Number of bullets currently in the magazine.
        /// </summary>
        public int currentAmmo;

        /// <summary>
        /// Total number of bullets remaining in reserve.
        /// </summary>
        public int totalAmmoLeft;

        /// <summary>
        /// Indicates whether the weapon is currently being reloaded.
        /// </summary>
        public bool isReloading;

        /// <summary>
        /// Initializes a new weapon state with default values from weapon data.
        /// </summary>
        /// <param name="weaponData">The weapon configuration to base the initial state on.</param>
        public WeaponState(WeaponData weaponData)
        {
            // Initialize with full magazine and calculate reserve ammo.
            currentAmmo = weaponData.magazineSize;
            totalAmmoLeft = weaponData.maxAmmo - weaponData.magazineSize;
            isReloading = false;
        }
    }
}