using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponRecoil : MonoBehaviour
    {
        [Header("- Recoil Settings")]
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private float returnSpeed = 10f;
        [SerializeField] private float recoilIntensity = 2f;

        private Vector3 _currentRotation;
        private Vector3 _targetRotation;

        private void Update()
        {
            // Smoothly return weapon to original position.
            UpdateRecoilRotation();
            ApplyRecoilRotation();
        }

        private void UpdateRecoilRotation()
        {
            // Gradually reduce target rotation.
            _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
        }

        private void ApplyRecoilRotation()
        {
            // Interpolate current rotation towards target.
            _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, Time.deltaTime * returnSpeed);
            weaponHolder.localRotation = Quaternion.Euler(_currentRotation);
        }

        public void ApplyRecoil(WeaponData weaponData)
        {
            // Add randomized recoil based on weapon data.
            _targetRotation += new Vector3(
                weaponData.recoilX * recoilIntensity,
                Random.Range(-weaponData.recoilY, weaponData.recoilY) * recoilIntensity,
                Random.Range(-weaponData.recoilZ, weaponData.recoilZ) * recoilIntensity
            );
        }
    }
}