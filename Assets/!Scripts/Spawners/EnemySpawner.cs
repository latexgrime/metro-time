using System.Collections;
using UnityEngine;

namespace _Scripts.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject spawnEffectPrefab;
        [SerializeField] private AudioClip spawnSound;
        [SerializeField] [Range(0f, 1f)] private float spawnSoundVolume = 0.8f;
        [SerializeField] private float spawnDelay = 0.5f;

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

        public void SpawnEnemies()
        {
            StartCoroutine(SpawnWithDelay());
        }

        private IEnumerator SpawnWithDelay()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (enemyPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, enemyPrefabs.Length);
                    GameObject enemy = Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

                    // VFX
                    if (spawnEffectPrefab != null)
                    {
                        Instantiate(spawnEffectPrefab, spawnPoint.position, Quaternion.identity);
                    }

                    // SFX
                    if (_audioSource != null && spawnSound != null)
                    {
                        _audioSource.PlayOneShot(spawnSound, spawnSoundVolume);
                    }

                    yield return new WaitForSeconds(spawnDelay);
                }
            }
        }

        public void SetSpawnPoints(Transform[] points)
        {
            spawnPoints = points;
        }

        public void SetEnemyPrefabs(GameObject[] prefabs)
        {
            enemyPrefabs = prefabs;
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
