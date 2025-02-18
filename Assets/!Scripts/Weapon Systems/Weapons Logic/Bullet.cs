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
            if (impactEffect == null) return;

            GameObject impact = Instantiate(
                impactEffect, 
                collision.contacts[0].point,
                Quaternion.LookRotation(collision.contacts[0].normal)
            );
            Destroy(impact, 2f);
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