using UnityEngine;

namespace _Scripts.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;

        public void SpawnEnemies()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (enemyPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, enemyPrefabs.Length);
                    Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
                }
            }
        }
    }
}