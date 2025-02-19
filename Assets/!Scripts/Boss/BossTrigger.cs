using UnityEngine;

namespace _Scripts.Boss
{
    public class BossTrigger : MonoBehaviour
    {
        [Header("- Boss Reference")]
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private Transform bossSpawnPoint;
    
        [Header("- Spawn Effects")]
        [SerializeField] private GameObject bossSpawnEffect;
    
        private bool bossSpawned = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!bossSpawned && other.CompareTag("PlayerCollider"))
            {
                bossSpawned = true;
                SpawnBoss();
            }
        }

        private void SpawnBoss()
        {
            if (bossPrefab != null && bossSpawnPoint != null)
            {
                GameObject bossInstance = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);

                // Play spawn effects [NEEDS IMPLEMENTATION].
                if (bossSpawnEffect != null)
                {
                    Instantiate(bossSpawnEffect, bossSpawnPoint.position, Quaternion.identity);
                }

                Debug.Log("Boss has spawned!");
            }
        }
    }
}