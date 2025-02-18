using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [Header("- Enemy Prefabs")]
    public GameObject stunEnemy;
    public GameObject slowEnemy;

    [Header("- NavMesh Agents")]
    public NavMeshAgent[] agent;
    public float range;

    [Header("- Spawn Settings")]
    public Transform centrePoint;
    public bool isEnabled = true; // Toggle enemy spawning.

    void Start()
    {
        if (isEnabled)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        StartCoroutine(TimerSpawnEnemies(10));
    }

    private void Update()
    {
        if (!isEnabled) return; // Stop enemy movement when spawner is disabled.

        if (!agent[0].isOnNavMesh || !agent[1].isOnNavMesh) return;

        if (agent[0].remainingDistance <= agent[0].stoppingDistance)
        {
            Vector3 point;
            if (Patrol(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent[0].SetDestination(point);
            }
        }

        if (agent[1].remainingDistance <= agent[1].stoppingDistance)
        {
            Vector3 point;
            if (Patrol(centrePoint.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent[1].SetDestination(point);
            }
        }
    }

    private bool Patrol(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    IEnumerator TimerSpawnEnemies(float waitTime)
    {
        yield return new WaitForSeconds(5);
        while (isEnabled) // Stop spawning when disabled.
        {
            if (gameObject.name == "Spawner_SlowEnemy")
            {
                Instantiate(slowEnemy);
                Vector3 pos = transform.position;
                slowEnemy.transform.position = pos;
                pos.z += 2.0f;
                if (pos.z >= 44f)
                    pos.z = 0;
                yield return new WaitForSeconds(waitTime);
            }
            else if (gameObject.name == "Spawner_StunEnemy")
            {
                Instantiate(stunEnemy);
                Vector3 posTwo = transform.position;
                stunEnemy.transform.position = posTwo;
                posTwo.z += 2.0f;
                if (posTwo.z >= 44f)
                    posTwo.z = 0;
                yield return new WaitForSeconds(waitTime);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
