using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class WeaponRecoil : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private float returnSpeed = 10f;
        [SerializeField] private float recoilIntensity = 2f;

        private Vector3 _currentRotation;
        private Vector3 _targetRotation;

        private void Update()
        {
            // Smooth return to original position.
            _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
            _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, Time.deltaTime * returnSpeed);
            weaponHolder.localRotation = Quaternion.Euler(_currentRotation);
        }

        public void ApplyRecoil(WeaponData weaponData)
        {
            _targetRotation += new Vector3(
                weaponData.recoilX * recoilIntensity,
                Random.Range(-weaponData.recoilY, weaponData.recoilY) * recoilIntensity,
                Random.Range(-weaponData.recoilZ, weaponData.recoilZ) * recoilIntensity
            );
        }
    }
}