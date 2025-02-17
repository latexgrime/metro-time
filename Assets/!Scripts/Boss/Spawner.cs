using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    
    // Update is called once per frame
    void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        StartCoroutine(TimerSpawnEnemies(2));
    }
    IEnumerator TimerSpawnEnemies(float waitTime)
    {
        while (true)
        {
            if (gameObject.name == "Spawner_SlowEnemy")
            {
                Instantiate(enemies[0]);
                Vector3 pos = enemies[0].transform.position;
                pos.z += 2.0f;
                if (pos.z >= 44f)
                    pos.z = 0;
                enemies[0].transform.position = pos;
            }
            if (gameObject.name == "Spawner_RootEnemy")
            {
                Instantiate(enemies[1]);
                Vector3 posTwo = enemies[1].transform.position;
                posTwo.z += 2.0f;
                if (posTwo.z >= 44f)
                    posTwo.z = 0;
                enemies[0].transform.position = posTwo;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}
