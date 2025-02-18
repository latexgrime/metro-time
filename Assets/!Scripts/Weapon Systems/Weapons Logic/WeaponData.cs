using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public enum WeaponType
    {
        Automatic,
        SingleShot
    }

    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("- Weapon Info")]
        public string weaponName;
        public WeaponType weaponType;
        public GameObject weaponPrefab;
        
        [Header("- UI Elements")]
        public Sprite weaponIcon;
        public Color weaponIconTint = Color.white;

        [Header("- Weapon Stats")]
        public float damage = 10f;
        public float fireRate = 10f;
        public float range = 100f;
        public float spread = 0.1f;
        public int bulletsPerShot = 1;

        [Header("- Ammo")]
        public int maxAmmo = 30;
        public int magazineSize = 30;
        public float reloadTime = 2f;
        public bool isAutomatic;

        [Header("- Recoil")]
        public float recoilX = -2f;
        public float recoilY = 2f;
        public float recoilZ = 0.35f;

        [Header("- Effects")]
        public AudioClip shootSound;
        public AudioClip reloadSound;
        public ParticleSystem muzzleFlashPrefab;
        public GameObject impactEffectPrefab;
        
        [Header("- Bullet Properties")]
        public GameObject bulletPrefab;
        public float bulletSpeed = 30f;
        public float bulletLifetime = 3f;
        public bool usePhysicalBullets = true;
        
        [Header("- Animations")]
        public AnimationClip idleAnimation;
        public AnimationClip walkAnimation;
        public AnimationClip sprintAnimation;
        public AnimationClip reloadAnimation;
        public AnimationClip fireAnimation;
        public float reloadAnimationSpeed = 1f;
        public float fireAnimationSpeed = 1f;
        public float walkAnimationSpeed = 1f;
        public float sprintAnimationSpeed = 1f;
    }
}