using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
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

            // Instantiate muzzle flash if prefab exists.
            if (weaponData.muzzleFlashPrefab != null)
                _muzzleFlash = Instantiate(weaponData.muzzleFlashPrefab, _shootPoint);

            // Initialize animator.
            InitializeAnimator();
        }

        private void InitializeAnimator()
        {
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
                Debug.LogError("Default Weapon Animator Controller not found in Resources folder.");
                return;
            }

            // Create an override controller.
            AnimatorOverrideController overrideController = new AnimatorOverrideController(baseController);
            
            // Override animations if provided.
            OverrideAnimationClip(overrideController, "Reload", weaponData.reloadAnimation);
            OverrideAnimationClip(overrideController, "Fire", weaponData.fireAnimation);
            OverrideAnimationClip(overrideController, "Idle", weaponData.idleAnimation);
            OverrideAnimationClip(overrideController, "Walk", weaponData.walkAnimation);
            OverrideAnimationClip(overrideController, "Sprint", weaponData.sprintAnimation);

            _animator.runtimeAnimatorController = overrideController;
        }

        private void OverrideAnimationClip(AnimatorOverrideController controller, string animationName, AnimationClip clip)
        {
            if (clip != null)
            {
                controller[animationName] = clip;
            }
        }

        public WeaponData GetWeaponData() => weaponData;

        public Transform GetShootPoint() => _shootPoint;

        public void PlayMuzzleFlash()
        {
            if (_muzzleFlash != null) 
                _muzzleFlash.Play();
        }

        public Animator GetAnimator() => _animator;
        
        public void UpdateMovementState(bool isSprinting, float movementSpeed)
        {
            if (_animator == null) return;

            // Set movement parameters.
            _animator.SetBool("IsSprinting", isSprinting);
            _animator.SetFloat("MovementSpeed", movementSpeed);
        }
    }
}