using UnityEngine;

namespace _Scripts.Boss
{
    public class EffectDestroyer : MonoBehaviour
    {
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private bool checkParticles = true;
        
        private ParticleSystem[] _particleSystems;
        private float _startTime;
        
        private void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            _startTime = Time.time;
        }
        
        private void Update()
        {
            // If we should check particles and have particle systems.
            if (checkParticles && _particleSystems.Length > 0)
            {
                bool allStopped = true;
                
                // Check if all particle systems have stopped.
                foreach (var ps in _particleSystems)
                {
                    if (ps.isPlaying && ps.particleCount > 0)
                    {
                        allStopped = false;
                        break;
                    }
                }
                
                // Destroy if all systems have finished or lifetime exceeded.
                if (allStopped || (Time.time - _startTime > lifetime))
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // Simple timer-based destruction.
                if (Time.time - _startTime > lifetime)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}