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
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Handle impact effect.
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, collision.contacts[0].point,
                    Quaternion.LookRotation(collision.contacts[0].normal));
                Destroy(impact, 2f);
            }

            // Handle shield damage using IShieldable.
            IShieldable shieldable = collision.gameObject.GetComponent<IShieldable>();
            if (shieldable != null)
            {
                shieldable.TakeShieldDamage(damage);
            }

            // Debug hit information.
            Debug.Log($"Bullet hit: {collision.gameObject.name} at {collision.contacts[0].point}");

            // Destroy the bullet.
            Destroy(gameObject);
        }
    }
}