namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    /// <summary>
    ///     This script is in charge of mantaining the ammo data.
    /// </summary>
    
    [System.Serializable]
    public class WeaponState
    {
        public int currentAmmo;
        public int totalAmmoLeft;
        public bool isReloading;

        public WeaponState(WeaponData weaponData)
        {
            // Initialize with full magazine and calculate reserve ammo.
            currentAmmo = weaponData.magazineSize;
            totalAmmoLeft = weaponData.maxAmmo - weaponData.magazineSize;
            isReloading = false;
        }
    }
}