using UnityEngine;

namespace _Scripts.Weapon_Systems
{
    public class WeaponRecoil : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private float returnSpeed = 10f;

        private Vector3 _currentRotation;
        private Vector3 _targetRotation;

        private void Update()
        {
            _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, Time.deltaTime * returnSpeed);
            _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, Time.deltaTime * returnSpeed);
            weaponHolder.localRotation = Quaternion.Euler(_currentRotation);
        }

        public void ApplyRecoil(WeaponData weaponData)
        {
            _targetRotation += new Vector3(
                weaponData.recoilX,
                Random.Range(-weaponData.recoilY, weaponData.recoilY),
                Random.Range(-weaponData.recoilZ, weaponData.recoilZ)
            );
        }
    }
}