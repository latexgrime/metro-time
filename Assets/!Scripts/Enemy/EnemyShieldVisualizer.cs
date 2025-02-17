using UnityEngine;

namespace _Scripts.Enemy
{
    public class EnemyShieldVisualizer : MonoBehaviour
    {
        [Header("Shield Visualization")]
        [SerializeField] private Color shieldColor = new Color(0.3f, 0.7f, 1f, 0.5f);
        [SerializeField] private float minAlpha = 0f;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.2f;
        [SerializeField] private float shieldScale = 1.2f; // How much bigger than the sprite
        
        private BaseEnemy _enemy;
        private SpriteRenderer _shieldRenderer;
        private GameObject _shieldObject;
        private Transform _spriteTransform; // Reference to the child Sprite object

        private void Start()
        {
            _enemy = GetComponent<BaseEnemy>();
            
            // Find the Sprite child object (which has the SpriteRenderer)
            Transform spriteChild = transform.Find("Sprite");
            if (spriteChild != null)
            {
                _spriteTransform = spriteChild;
            }
            
            // Create shield sprite object
            CreateShieldSprite();
        }

        private void CreateShieldSprite()
        {
            _shieldObject = new GameObject("Shield");
            _shieldObject.transform.SetParent(_spriteTransform != null ? _spriteTransform : transform);
            _shieldObject.transform.localPosition = Vector3.zero;
            
            // Add sprite renderer
            _shieldRenderer = _shieldObject.AddComponent<SpriteRenderer>();
            
            // Copy sprite properties from the enemy sprite
            SpriteRenderer enemyRenderer = (_spriteTransform != null ? _spriteTransform : transform)
                .GetComponent<SpriteRenderer>();
            
            if (enemyRenderer != null)
            {
                _shieldRenderer.sprite = enemyRenderer.sprite;
                _shieldRenderer.sortingOrder = enemyRenderer.sortingOrder - 1; // Render behind the enemy
            }
            
            // Set shield scale
            _shieldObject.transform.localScale = Vector3.one * shieldScale;
            
            // Add SpriteBillboard component if the enemy has one
            if (GetComponentInChildren<SpriteBillboard>() != null)
            {
                SpriteBillboard shieldBillboard = _shieldObject.AddComponent<SpriteBillboard>();
                // Copy any specific settings from the enemy's SpriteBillboard if needed
            }
            
            // Set initial material properties
            _shieldRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _shieldRenderer.color = shieldColor;
        }

        private void Update()
        {
            if (_enemy == null || _shieldRenderer == null) return;

            float shieldPercentage = _enemy.GetCurrentShield() / _enemy.GetMaxShield();
            
            // Calculate base alpha
            float baseAlpha = Mathf.Lerp(minAlpha, shieldColor.a, shieldPercentage);
            
            // Add pulsing effect when shield is low (below 30%)
            float pulseAlpha = 0f;
            if (shieldPercentage < 0.3f && !_enemy.IsDeactivated())
            {
                pulseAlpha = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            }
            
            // Update shield color and alpha
            Color currentColor = shieldColor;
            currentColor.a = baseAlpha + pulseAlpha;
            _shieldRenderer.color = currentColor;
            
            // Enable/disable shield renderer based on enemy state
            _shieldRenderer.enabled = !_enemy.IsDeactivated();
        }

        private void OnDestroy()
        {
            if (_shieldObject != null)
            {
                Destroy(_shieldObject);
            }
        }
    }
}