using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Boss
{
    public class ProjectilePool : MonoBehaviour
    {
        [System.Serializable]
        public class ProjectilePoolItem
        {
            public GameObject prefab;
            public int initialPoolSize = 50;
            public int maxPoolSize = 250;
            [HideInInspector] public Queue<GameObject> pool;
        }

        [Header("- Pool Settings")]
        [SerializeField] private List<ProjectilePoolItem> projectileTypes = new List<ProjectilePoolItem>();
        [SerializeField] private bool autoExpandPool = true;
        [SerializeField] private Transform poolContainer;

        // Dictionary to quickly find the right pool by name.
        private Dictionary<string, ProjectilePoolItem> _poolDictionary;

        private void Awake()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            _poolDictionary = new Dictionary<string, ProjectilePoolItem>();
        
            if (poolContainer == null)
            {
                poolContainer = new GameObject("ProjectilePoolContainer").transform;
                poolContainer.SetParent(transform);
            }

            // Initialize a pool for each projectile type.
            foreach (ProjectilePoolItem item in projectileTypes)
            {
                if (item.prefab == null) continue;

                string key = item.prefab.name;
                item.pool = new Queue<GameObject>();
                _poolDictionary[key] = item;

                // Create initial pool objects.
                for (int i = 0; i < item.initialPoolSize; i++)
                {
                    CreateNewPoolObject(item, key);
                }
            
                Debug.Log($"Initialized pool for {key} with {item.initialPoolSize} projectiles");
            }
        }

        private GameObject CreateNewPoolObject(ProjectilePoolItem item, string key)
        {
            GameObject projectile = Instantiate(item.prefab, poolContainer);
            projectile.name = key; // This is to clean the name lol.
            projectile.SetActive(false);
        
            // Make sure it has the PooledProjectile component (more dummy proofing).
            if (projectile.GetComponent<PooledProjectile>() == null)
            {
                projectile.AddComponent<PooledProjectile>();
            }
        
            item.pool.Enqueue(projectile);
            return projectile;
        }

        public GameObject GetProjectile(string projectileType, Vector3 position, Quaternion rotation)
        {
            // Clean up projectile name in case it was passed with (Clone).
            projectileType = projectileType.Replace("(Clone)", "").Trim();
        
            if (!_poolDictionary.ContainsKey(projectileType))
            {
                Debug.LogError($"Projectile type {projectileType} not found in pool!");
                return null;
            }

            ProjectilePoolItem poolItem = _poolDictionary[projectileType];
        
            if (poolItem.pool.Count == 0)
            {
                if (autoExpandPool && poolItem.pool.Count < poolItem.maxPoolSize)
                {
                    Debug.Log($"Expanding pool for {projectileType}. Current size: {poolItem.pool.Count}");
                    return CreateNewPoolObject(poolItem, projectileType);
                }
                else
                {
                    Debug.LogWarning($"No available projectiles of type {projectileType}! Max pool size reached.");
                    return null;
                }
            }

            GameObject projectile = poolItem.pool.Dequeue();
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
            projectile.SetActive(true);

            return projectile;
        }

        public void ReturnProjectile(GameObject projectile)
        {
            string key = projectile.name.Replace("(Clone)", "").Trim();

            if (_poolDictionary.ContainsKey(key))
            {
                projectile.SetActive(false);
                _poolDictionary[key].pool.Enqueue(projectile);
            }
            else
            {
                Debug.LogWarning($"Tried to return a projectile of unknown type: {key}");
                Destroy(projectile);
            }
        }

        // For debugging.
        public void LogPoolStatus()
        {
            foreach (var key in _poolDictionary.Keys)
            {
                Debug.Log($"Pool '{key}': {_poolDictionary[key].pool.Count} available of {_poolDictionary[key].maxPoolSize} max");
            }
        }
    }
}