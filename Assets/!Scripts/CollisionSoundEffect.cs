using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace NALEO._Scripts
{
    public class ObjectCollisionHandler : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] AudioClip collisionSfx;
        // The velocity needed to trigger sound. This is a workaround because even if the object is standing still, it still generates collisions.
        [SerializeField] private float collisionVelocityThreshold = 0.5f;

        private float _defaultVolume;
        private float _defaultPitch;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _defaultVolume = _audioSource.volume;
            _defaultPitch = _audioSource.pitch;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.relativeVelocity.magnitude >= collisionVelocityThreshold)
            {
                PlayCollisionSfx();
            }
        }

        private void PlayCollisionSfx()
        {
            _audioSource.pitch = Random.Range(_defaultPitch - 0.1f, _defaultPitch + 0.1f);
            _audioSource.volume = Random.Range(_defaultVolume - 0.2f, _defaultVolume + 0.2f);
            _audioSource.PlayOneShot(collisionSfx);
        }
    }
}