using UnityEngine;

namespace _Scripts.Spawners
{
    public class AmmoSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject ammoPrefab;
        [SerializeField] private Transform[] spawnPoints;

        public void SpawnAmmo()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(ammoPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}