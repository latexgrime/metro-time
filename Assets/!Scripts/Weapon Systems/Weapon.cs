using UnityEngine;

namespace _Scripts.Weapon_Systems
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;

        private ParticleSystem _muzzleFlash;
        private Transform _shootPoint;

        public WeaponData GetWeaponData()
        {
            return weaponData;
        }

        private void Awake()
        {
            // Find or create muzzle flash point.
            _shootPoint = transform.Find("ShootPoint");
            if (_shootPoint == null)
            {
                _shootPoint = new GameObject("ShootPoint").transform;
                _shootPoint.SetParent(transform);
                _shootPoint.localPosition = Vector3.forward;
            }

            if (weaponData.muzzleFlashPrefab != null)
                _muzzleFlash = Instantiate(weaponData.muzzleFlashPrefab, _shootPoint);
        }

        public Transform GetShootPoint()
        {
            return _shootPoint;
        }

        public void PlayMuzzleFlash()
        {
            if (_muzzleFlash != null) _muzzleFlash.Play();
        }
    }
}