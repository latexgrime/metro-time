using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject[] bullets;
    private int random;
    public Transform bulletPos;

    public Transform player;

    public float bulletVelocity = 30f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShootPlayer(2.0f));
        random = Random.Range(0, 1);
    }

    IEnumerator ShootPlayer(float time)
    {
        Vector3 shootingDirection = whichDirection().normalized;
        while (true)
        {
            Instantiate(bullets[random], bulletPos);
            bullets[random].GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
            yield return new WaitForSeconds(time);
        }
    }

    private Vector3 whichDirection()
    {
        Vector3 direction = player.position - bulletPos.position;
        return direction;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
