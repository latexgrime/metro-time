using System.Collections;
using _Scripts.Player.Movement;
using _Scripts.Status_System.UI;
using UnityEngine;

namespace _Scripts.Status_System
{
    public class StatusEffectManager : MonoBehaviour
    {
        [Header("- Dependencies")]
        [SerializeField] private StatusEffectUI statusUI;
        private AudioSource _audioSource;
        private PlayerMovement _playerMovement;
        private StatusEffectHandler _statusEffectHandler;

        [Header("- Status Thresholds")]
        [SerializeField] private float maxStatusValue = 100f;
        [SerializeField] private float barDecayRate = 0.5f;

        [Header("- Audio")]
        [SerializeField] private AudioClip stunAppliedSound;
        [SerializeField] private AudioClip slowAppliedSound;
        [SerializeField] private AudioClip statusBuildupSound;

        private float _currentStunValue;
        private float _currentSlowValue;
        private bool _isStunned;
        private bool _isSlowed;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerMovement = GetComponent<PlayerMovement>();
            _statusEffectHandler = GetComponent<StatusEffectHandler>();
        }

        private void Update()
        {
            DecayStatusValues();
            UpdateUI();
            CheckStatusEffects();
        }

        private void DecayStatusValues()
        {
            if (!_isStunned)
                _currentStunValue = Mathf.Max(0, _currentStunValue - barDecayRate * Time.deltaTime);

            if (!_isSlowed)
                _currentSlowValue = Mathf.Max(0, _currentSlowValue - barDecayRate * Time.deltaTime);
        }

        private void UpdateUI()
        {
            if (statusUI == null) return;
            
            statusUI.UpdateStunBar(_currentStunValue);
            statusUI.UpdateSlowBar(_currentSlowValue);
            statusUI.SetStunIconActive(_isStunned);
            statusUI.SetSlowIconActive(_isSlowed);
        }

        private void CheckStatusEffects()
        {
            if (_currentStunValue >= maxStatusValue && !_isStunned)
                ApplyStunEffect();

            if (_currentSlowValue >= maxStatusValue && !_isSlowed)
                ApplySlowEffect();
        }

        public void AddStunBuildup(float amount)
        {
            if (_isStunned) return;

            _currentStunValue = Mathf.Min(_currentStunValue + amount, maxStatusValue);
            PlayBuildupSound();
        }

        public void AddSlowBuildup(float amount)
        {
            if (_isSlowed) return;

            _currentSlowValue = Mathf.Min(_currentSlowValue + amount, maxStatusValue);
            PlayBuildupSound();
        }

        private void PlayBuildupSound()
        {
            if (_audioSource != null && statusBuildupSound != null)
                _audioSource.PlayOneShot(statusBuildupSound, 0.5f);
        }

        private void ApplyStunEffect()
        {
            _isStunned = true;
            if (_audioSource != null && stunAppliedSound != null)
                _audioSource.PlayOneShot(stunAppliedSound);

            if (_statusEffectHandler != null)
                _statusEffectHandler.ApplyStun();

            StartCoroutine(ClearStunAfterDelay());
        }

        private void ApplySlowEffect()
        {
            _isSlowed = true;
            if (_audioSource != null && slowAppliedSound != null)
                _audioSource.PlayOneShot(slowAppliedSound);

            if (_statusEffectHandler != null)
                _statusEffectHandler.ApplySlowdown();

            StartCoroutine(ClearSlowAfterDelay());
        }

        private IEnumerator ClearStunAfterDelay()
        {
            yield return new WaitForSeconds(_statusEffectHandler.StunDuration);
            _isStunned = false;
            _currentStunValue = 0;
            UpdateUI();
        }

        private IEnumerator ClearSlowAfterDelay()
        {
            yield return new WaitForSeconds(_statusEffectHandler.SlowDuration);
            _isSlowed = false;
            _currentSlowValue = 0;
            UpdateUI();
        }
    }
}