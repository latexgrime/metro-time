using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform[] points;

    public int targetPoint;

    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Patrolling();
    }

    private void Patrolling()
    {
        if (transform.position == points[targetPoint].position)
        {
            targetPoint++;
            if (targetPoint >= points.Length)
            {
                targetPoint = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, points[targetPoint].position, speed * Time.deltaTime);
    }
}
