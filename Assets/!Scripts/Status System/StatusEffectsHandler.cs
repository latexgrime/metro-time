using System.Collections;
using _Scripts.Player.Movement;
using UnityEngine;

namespace _Scripts.Status_System
{
    public class StatusEffectHandler : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private Rigidbody _rb;
        private AudioSource _audioSource;

        [Header("- Effect Durations")]
        [SerializeField] private float stunDuration = 2f;
        [SerializeField] private float slowDuration = 3f;
        [SerializeField] private float slowIntensity = 0.5f;

        [Header("- Visual Effects")]
        [SerializeField] private ParticleSystem stunVFX;
        [SerializeField] private ParticleSystem slowVFX;
        
        public bool IsSlowed { get; private set; } = false;
        public bool IsStunned { get; private set; } = false;

        public float StunDuration => stunDuration;
        public float SlowDuration => slowDuration;

        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
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
            IsStunned = true;
            
            if (stunVFX != null)
                stunVFX.Play();

            if (_playerMovement != null)
                _playerMovement.enabled = false;

            if (_rb != null)
                _rb.linearVelocity = Vector3.zero;

            yield return new WaitForSeconds(stunDuration);
            IsStunned = false;

            if (_playerMovement != null)
                _playerMovement.enabled = true;

            if (stunVFX != null && stunVFX.isPlaying)
                stunVFX.Stop();
        }

        private IEnumerator SlowdownCoroutine()
        {
            IsSlowed = true;

            
            if (slowVFX != null)
                slowVFX.Play();

            if (_playerMovement != null)
                _playerMovement.SetMovementSpeedMultiplier(slowIntensity);

            yield return new WaitForSeconds(slowDuration);
            IsSlowed = false;

            if (_playerMovement != null)
                _playerMovement.SetMovementSpeedMultiplier(1f);

            if (slowVFX != null && slowVFX.isPlaying)
                slowVFX.Stop();
        }
    }
}