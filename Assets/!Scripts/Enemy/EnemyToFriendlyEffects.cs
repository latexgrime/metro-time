using UnityEngine;

namespace _Scripts.Enemy
{
    public class EnemyFriendlyEffects : MonoBehaviour
    {
        [Header("- Visual Settings")]
        [SerializeField] private Color friendlyTint = new Color(0.5f, 1f, 0.5f, 1f);
        [SerializeField] private GameObject friendlyParticlePrefab;
        [SerializeField] private float particleLifetime = 2f;

        [Header("- Audio Settings")]
        [SerializeField] private AudioClip friendlyTransformSound;
        [SerializeField] private float soundVolume = 1f;
        
        private BaseEnemy _enemy;
        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private Material _spriteMaterial;
        private Color _originalColor;
        
        private void Start()
        {
            _enemy = GetComponent<BaseEnemy>();
            _audioSource = GetComponent<AudioSource>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Set initial audio source properties.
            _audioSource.playOnAwake = false;
            _audioSource.volume = soundVolume;
            
            if (_spriteRenderer != null)
            {
                // Create a material instance to avoid affecting other sprites.
                _spriteMaterial = new Material(_spriteRenderer.material);
                _spriteRenderer.material = _spriteMaterial;
                _originalColor = _spriteMaterial.color;
            }
        }

        private void Update()
        {
            if (_enemy != null && _spriteRenderer != null)
            {
                _spriteMaterial.color = _enemy.IsFriendly() ? friendlyTint : _originalColor;
            }
        }

        private void OnBecameFriendly()
        {
            // Spawn particle effect.
            if (friendlyParticlePrefab != null)
            {
                GameObject particles = Instantiate(friendlyParticlePrefab, transform.position, Quaternion.identity);
                Destroy(particles, particleLifetime);
            }

            // Play transformation sound.
            if (friendlyTransformSound != null && _audioSource != null)
            {
                _audioSource.volume = soundVolume; 
                _audioSource.PlayOneShot(friendlyTransformSound);
            }
        }

        private void OnDestroy()
        {
            // Clean up the material instance.
            if (_spriteMaterial != null)
            {
                Destroy(_spriteMaterial);
            }
        }
    }
}