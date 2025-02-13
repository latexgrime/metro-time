using UnityEngine;

namespace _Scripts.Weapon_Systems
{
    public enum WeaponType
    {
        Pistol,
        Submachine,
        Rifle,
        Shotgun
    }

    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon Info")] public string weaponName;
        public WeaponType weaponType;
        public GameObject weaponPrefab;

        [Header("Weapon Stats")] public float damage = 10f;
        public float fireRate = 10f;
        public float range = 100f;
        public float spread = 0.1f;
        public int bulletsPerShot = 1;

        [Header("Ammo")] public int maxAmmo = 30;
        public int magazineSize = 30;
        public float reloadTime = 2f;
        public bool isAutomatic;

        [Header("Recoil")] public float recoilX = -2f;
        public float recoilY = 2f;
        public float recoilZ = 0.35f;

        [Header("Effects")] public AudioClip shootSound;
        public AudioClip reloadSound;
        public ParticleSystem muzzleFlashPrefab;
        public GameObject impactEffectPrefab;
    }
}