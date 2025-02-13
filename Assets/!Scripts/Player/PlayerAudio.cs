using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        private AudioSource _audioSource;
        private PlayerMovement _playerMovement;
        private float _stepTimer;
        private float _defaultPitch;
        private float _defaultVolume;

        [Header("Audio Clips")] [SerializeField]
        private AudioClip walkInSandSfx;

        [SerializeField] private AudioClip walkInWaterSfx;
        [SerializeField] private AudioClip walkInWoodSfx;
        [SerializeField] private AudioClip walkInRockSfx;
        [SerializeField] private AudioClip landSfx;
        [SerializeField] private AudioClip jumpSfx;
        [SerializeField] private AudioClip dashSfx;

        [Header("Step Intervals")] [SerializeField]
        private float stepIntervalForWalkSfx = 0.5f;

        [SerializeField] private float stepIntervalForRunSfx = 0.3f;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerMovement = GetComponent<PlayerMovement>();
            _defaultPitch = _audioSource.pitch;
            _defaultVolume = _audioSource.volume;
        }

        private void Update()
        {
            HandleMovementSounds();
        }

        private void HandleMovementSounds()
        {
            switch (_playerMovement.CurrentState)
            {
                case PlayerState.Walking:
                case PlayerState.Sprinting:
                    HandleFootstepSounds();
                    break;

                case PlayerState.InAir:
                    _stepTimer = 0f; // Reset step timer when in air
                    break;
            }
        }

        public void PlayStateEnterSound(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Jumping:
                    PlayJumpSound();
                    break;
                case PlayerState.Dashing:
                    PlayDashSound();
                    break;
            }
        }

        public void PlayStateExitSound(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.InAir:
                    if (_playerMovement.IsGrounded()) // Only play landing sound when actually landing
                        PlayLandingSound();
                    break;
            }
        }

        private void HandleFootstepSounds()
        {
            if (_playerMovement.IsGrounded())
            {
                _stepTimer += Time.deltaTime;
                var currentInterval = _playerMovement.CurrentState == PlayerState.Sprinting
                    ? stepIntervalForRunSfx
                    : stepIntervalForWalkSfx;

                if (_stepTimer >= currentInterval)
                {
                    PlayFootstepSound();
                    _stepTimer = 0f;
                }
            }
            else
            {
                _stepTimer = 0f;
            }
        }

        private void PlayFootstepSound()
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.volume = Random.Range(0.8f, 1.2f);
            _audioSource.PlayOneShot(GetSurfaceSound());
        }

        private void PlayJumpSound()
        {
            _audioSource.PlayOneShot(jumpSfx);
        }

        private void PlayDashSound()
        {
            _audioSource.PlayOneShot(dashSfx);
        }

        private void PlayLandingSound()
        {
            _audioSource.PlayOneShot(landSfx);
        }

        private AudioClip GetSurfaceSound()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, _playerMovement.playerHeight * 1.5f,
                    _playerMovement.GroundLayer))
                switch (hit.transform.tag)
                {
                    case "Sand": return walkInSandSfx;
                    case "Wood": return walkInWoodSfx;
                    case "Water": return walkInWaterSfx;
                    case "Rock": return walkInRockSfx;
                }

            return walkInRockSfx;
        }
    }
}