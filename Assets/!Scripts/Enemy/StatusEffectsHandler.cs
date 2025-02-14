using UnityEngine;
using System.Collections;
using _Scripts.Player;
using _Scripts.Player.Movement;

namespace _Scripts.StatusSystem
{
    public class StatusEffectHandler : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private Rigidbody _rb;

        [Header("Effect Durations")]
        [SerializeField] private float stunDuration = 2f;
        [SerializeField] private float slowDuration = 3f;
        [SerializeField] private float slowIntensity = 0.5f;

        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _rb = GetComponent<Rigidbody>();
        }

        public void ApplyStun()
        {
            StartCoroutine(StunCoroutine());
        }

        public void ApplySlowdown()
        {
            StartCoroutine(SlowdownCoroutine());
        }

        private IEnumerator StunCoroutine()
        {
            // Apply stun
            _playerMovement.SetMovementEnabled(false);
            _rb.linearVelocity = Vector3.zero;

            // Wait for duration
            yield return new WaitForSeconds(stunDuration);

            // Remove stun
            _playerMovement.SetMovementEnabled(true);
        }

        private IEnumerator SlowdownCoroutine()
        {
            // Apply slowdown
            _playerMovement.SetMovementSpeedMultiplier(1f - slowIntensity);

            // Wait for duration
            yield return new WaitForSeconds(slowDuration);

            // Remove slowdown
            _playerMovement.SetMovementSpeedMultiplier(1f);
        }
    }
}