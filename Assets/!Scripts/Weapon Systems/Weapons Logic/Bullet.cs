using _Scripts.Enemy.Interfaces;
using UnityEngine;

namespace _Scripts.Weapon_Systems.Weapons_Logic
{
    public class Bullet : MonoBehaviour
    {
        [Header("- Bullet Properties")]
        public float damage;
        public float speed;
        public float lifetime = 3f;
        [SerializeField] private float impactEffectRotationOffset = 0f;

        [Header("- Effects")]
        public GameObject impactEffect;

        private void Start()
        {
            // Destroy bullet after specified lifetime.
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Handle impact effect.
            CreateImpactEffect(collision);

            // Handle shield damage.
            ApplyShieldDamage(collision);

            // Destroy the bullet.
            Destroy(gameObject);
        }

        private void CreateImpactEffect(Collision collision)
        {
            if (impactEffect != null)
            {
                // Create base rotation aligned with surface normal.
                Quaternion baseRotation = Quaternion.LookRotation(collision.contacts[0].normal);
        
                // Apply additional rotation.
                Quaternion offsetRotation = baseRotation * Quaternion.Euler(impactEffectRotationOffset, 0, 0);

                GameObject impact = Instantiate(impactEffect, collision.contacts[0].point,
                    offsetRotation);
                Destroy(impact, 2f);
            }
        }

        private void ApplyShieldDamage(Collision collision)
        {
            IShieldable shieldable = collision.gameObject.GetComponent<IShieldable>();
            if (shieldable != null)
            {
                shieldable.TakeShieldDamage(damage);
            }
        }
    }
}