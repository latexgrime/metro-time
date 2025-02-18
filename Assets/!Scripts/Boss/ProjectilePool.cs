using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [Header("- Pool Settings")]
    [SerializeField] private List<GameObject> projectilePrefabs;
    [SerializeField] private int poolSize = 20;

    private Dictionary<string, Queue<GameObject>> _projectilePools;

    private void Awake()
    {
        _projectilePools = new Dictionary<string, Queue<GameObject>>();

        // Initialize a pool for each projectile type.
        foreach (GameObject prefab in projectilePrefabs)
        {
            string key = prefab.name;
            _projectilePools[key] = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectile = Instantiate(prefab);
                projectile.SetActive(false);
                _projectilePools[key].Enqueue(projectile);
            }
        }
    }

    public GameObject GetProjectile(string projectileType, Vector3 position, Quaternion rotation)
    {
        if (!_projectilePools.ContainsKey(projectileType) || _projectilePools[projectileType].Count == 0)
        {
            Debug.LogWarning($"No available projectiles of type {projectileType}! Consider increasing pool size.");
            return null;
        }

        GameObject projectile = _projectilePools[projectileType].Dequeue();
        projectile.transform.position = position;
        projectile.transform.rotation = rotation;
        projectile.SetActive(true);

        return projectile;
    }

    public void ReturnProjectile(GameObject projectile)
    {
        string key = projectile.name.Replace("(Clone)", "").Trim();

        if (_projectilePools.ContainsKey(key))
        {
            projectile.SetActive(false);
            _projectilePools[key].Enqueue(projectile);
        }
        else
        {
            Debug.LogWarning($"Tried to return a projectile of unknown type: {key}");
            Destroy(projectile);
        }
    }
}
