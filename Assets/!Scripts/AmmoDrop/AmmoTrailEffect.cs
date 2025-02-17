using UnityEngine;

namespace _Scripts.AmmoDrop
{
    public class AmmoTrailEffect : MonoBehaviour
    {
        [Header("- Trail Settings")]
        [SerializeField] private float trailTime = 0.5f;
        [SerializeField] private Gradient trailColor;
        [SerializeField] private AnimationCurve widthCurve;
        [SerializeField] private float startWidth = 0.5f;
        [SerializeField] private float endWidth = 0f;
        
        private TrailRenderer _trailRenderer;
        private bool _isInitialized = false;

        private void Start()
        {
            InitializeTrail();
        }

        private void InitializeTrail()
        {
            if (_isInitialized) return;

            // Add TrailRenderer component.
            _trailRenderer = gameObject.AddComponent<TrailRenderer>();
            
            // Configure trail.
            _trailRenderer.time = trailTime;
            _trailRenderer.startWidth = startWidth;
            _trailRenderer.endWidth = endWidth;
            _trailRenderer.colorGradient = trailColor;
            _trailRenderer.widthCurve = widthCurve;
            
            // Material settings.
            _trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _trailRenderer.generateLightingData = false;
            _trailRenderer.autodestruct = false;

            _isInitialized = true;
        }

        public void EnableTrail(bool enable)
        {
            if (!_isInitialized) InitializeTrail();
            _trailRenderer.enabled = enable;
        }
    }
}