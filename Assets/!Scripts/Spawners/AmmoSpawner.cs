using UnityEngine;

namespace _Scripts.Spawners
{
    public class AmmoSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject ammoPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject spawnEffectPrefab;
        [SerializeField] private AudioClip spawnSound;
        [SerializeField] [Range(0f, 1f)] private float spawnSoundVolume = 0.8f;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 1f;
                _audioSource.rolloffMode = AudioRolloffMode.Linear;
                _audioSource.minDistance = 5f;
                _audioSource.maxDistance = 50f;
            }
        }

        public void SpawnAmmo()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(ammoPrefab, spawnPoint.position, Quaternion.identity);

                // VFX.
                if (spawnEffectPrefab != null)
                {
                    Instantiate(spawnEffectPrefab, spawnPoint.position, Quaternion.identity);
                }

                // SFX.
                if (_audioSource != null && spawnSound != null)
                {
                    _audioSource.PlayOneShot(spawnSound, spawnSoundVolume);
                }
            }
        }

        public void SetSpawnPoints(Transform[] points)
        {
            spawnPoints = points;
        }

        public void SetAmmoPrefab(GameObject prefab)
        {
            ammoPrefab = prefab;
        }

        public void SetSpawnEffect(GameObject effectPrefab)
        {
            spawnEffectPrefab = effectPrefab;
        }

        public void SetSpawnSound(AudioClip soundClip, float volume)
        {
            spawnSound = soundClip;
            spawnSoundVolume = volume;
        }
    }
}
