using UnityEngine;

namespace _Scripts.Weapon_Systems
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        
        private Animator _animator;
        private ParticleSystem _muzzleFlash;
        private Transform _shootPoint;

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

            // Get or add Animator component.
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                _animator = gameObject.AddComponent<Animator>();
            }

            // Load the base Animator Controller.
            RuntimeAnimatorController baseController = 
                Resources.Load<RuntimeAnimatorController>("DefaultWeaponAnimator");
            
            if (baseController == null)
            {
                Debug.LogError("Default Weapon Animator Controller not found in Resources folder!");
                return;
            }

            // Create an override controller.
            AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);
            
            // Override animations if provided.
            if (weaponData.reloadAnimation != null)
                overrideController["Reload"] = weaponData.reloadAnimation;
            if (weaponData.fireAnimation != null)
                overrideController["Fire"] = weaponData.fireAnimation;
            if (weaponData.idleAnimation != null)
                overrideController["Idle"] = weaponData.idleAnimation;
            if (weaponData.walkAnimation != null)
                overrideController["Walk"] = weaponData.walkAnimation;
            if (weaponData.sprintAnimation != null)
                overrideController["Sprint"] = weaponData.sprintAnimation;

            _animator.runtimeAnimatorController = overrideController;
        }

        public WeaponData GetWeaponData()
        {
            return weaponData;
        }

        public Transform GetShootPoint()
        {
            return _shootPoint;
        }

        public void PlayMuzzleFlash()
        {
            if (_muzzleFlash != null) _muzzleFlash.Play();
            
            // Trigger fire animation.
            if (_animator != null)
            {
                _animator.SetTrigger("Fire");
            }
        }

        // Method to get the Animator for external access if needed for other scripts (most probably the handler in the future if we implement more).
        public Animator GetAnimator()
        {
            return _animator;
        }
        
        // This is to update the movement states in the animator.
        public void UpdateMovementState(bool isSprinting, float movementSpeed)
        {
            if (_animator == null) return;

            // Set movement parameters
            _animator.SetBool("IsSprinting", isSprinting);
            _animator.SetFloat("MovementSpeed", movementSpeed);
        }
    }
}